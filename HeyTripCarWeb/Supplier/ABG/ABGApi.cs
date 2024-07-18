using CommonCore.Mapper;
using Dapper;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;
using HeyTripCarWeb.Supplier.ABG.Models.Dtos;
using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using HeyTripCarWeb.Supplier.ABG.Util;
using HeyTripCarWeb.Supplier.ACE.Util;
using HeyTripCarWeb.Supplier.Sixt;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Twilio.Rest.Trunking.V1;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.ABG
{
    /// <summary>
    /// 结算价和计佣价和代收都是base rate。
    /// 结算价包含：EstimatedTotalAmount。
    /// 结算价字段：EstimatedTotalAmount。
    /// 存在两个品牌 Avis Budget 分别存在两种支付方式 到付还是预付款
    /// AUD(澳大利亚元)
    ///
    /// 支持门店码查询
    /// 取消政策：取车前免费取消  noshow 取 60 EUR
    /// 燃油政策为FullToFull
    /// 政策方面接口有返回 另外根据FTP文件 落地 信用卡要求个数政策  YoungDriver政策.
    /// FTP文件落地 location(locs.dat) locationOperationTimes(Locs_hrs.dat)  YoungDriver(YoungDriver.dat)
    /// CreditCard信用卡要求个数(CreditCard.dat)
    /// 押金：好像还需要推进 是一个范围？ 押金用我们标准的万能范围：300-5000USD
    ///
    /// </summary>
    public class ABGApi : IABGApi
    {
        private readonly IRepository<ABGLocation> _locRepository;
        private readonly IRepository<ABG_CreditCardPolicy> _cardRepository;
        private readonly IRepository<AbgYoungDriver> _yourDriverRepository;
        private readonly ABGAppSetting _setting;
        private readonly IMapper _mapper;
        private readonly IRepository<CarLocationSupplier> _supplierCatRe;
        private readonly IRepository<CarCity> _CatCityRe;
        private readonly IRepository<ABGCarProReservation> _proOrdRepository;
        private readonly IRepository<ABGRateCache> _rateCacheRepository;
        private readonly IServiceProvider _serviceProvider;

        public ABGApi(IOptions<ABGAppSetting> options, IMapper mapper, IRepository<ABGLocation> locRepository,
            IRepository<ABGCarProReservation> proOrdRepository, IRepository<ABGRateCache> rateCacheRepository,
            IServiceProvider serviceProvider, IRepository<ABG_CreditCardPolicy> cardRepository,
            IRepository<AbgYoungDriver> yourDriverRepository, IRepository<CarLocationSupplier> supplierCatRe,
            IRepository<CarCity> catCityRe)
        {
            _setting = options.Value;
            _mapper = mapper;
            _locRepository = locRepository;
            _proOrdRepository = proOrdRepository;
            _rateCacheRepository = rateCacheRepository;
            _serviceProvider = serviceProvider;
            _cardRepository = cardRepository;
            _yourDriverRepository = yourDriverRepository;
            _supplierCatRe = supplierCatRe;
            _CatCityRe = catCityRe;
        }

        #region 原始接口

        /// <summary>
        /// 获取所有门店
        /// </summary>
        /// <returns></returns>
        public async Task<List<CarLocationSupplier>> GetAllLocation()
        {
            var spLoc = ABGCacheInstance.Instance.GetAllItems();
            if (spLoc.Count == 0)
            {
                spLoc = await _supplierCatRe.GetListBySqlAsync("select * from CarRental.dbo.Car_Location_Suppliers where supplier =@supplier and status = 1", new { supplier = (int)EnumCarSupplier.ABG });
                ABGCacheInstance.Instance.SetLocation(spLoc);
            }
            return spLoc;
        }

        public async Task<List<AbgYoungDriver>> GetAllYoungDriverAsync()
        {
            var spLoc = ABGCacheInstance.Instance.GetAllAbgYoungDriver();
            if (spLoc.Count == 0)
            {
                spLoc = await _yourDriverRepository.GetAllAsync();
                ABGCacheInstance.Instance.SetYoungDriver(spLoc);
            }
            return spLoc;
        }

        /// <summary>
        /// 查询车辆集合。
        /// 支持：IATA/AirportID/CityID/Geo
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicleExtend>> VehAvailRate(ABG_OTA_VehAvailRateRQ availRateRQ, QueryDto dto, int timeout = 45000)
        {
            var supplierInfo = dto.SupplierInfo;

            var driverList = dto.youngDriverList;
            List<StdVehicleExtend> result = new List<StdVehicleExtend>();
            var res = BuildEnvelope(new CommonRequest { OTA_VehAvailRateRQ = availRateRQ, Type = ApiEnum.List });
            var model = ABGXmlHelper.GetResponse<ABG_OTA_VehAvailRateRS>(res);
            if (model.Errors != null && model.Errors?.ErrorList.Count > 0)
            {
                Log.Information($"Apiresult:{supplierInfo.DefaultIATA}{string.Join(",", model.Errors?.ErrorList.Select(n => n.Message))}");
                return result;
            }
            if (model == null)
            {
                return result;
            }
            var rentCore = model.VehAvailRSCore.VehRentalCore;
            var avails = model.VehAvailRSCore.VehVendorAvails.VehVendorAvail;

            foreach (var avail in avails)
            {
                var loc = avail.Info.LocationDetails;
                var veh = avail.VehAvails.VehAvail;
                Log.Information($"取到{veh.Count}数据");
                foreach (var ve in veh)
                {
                    var veCore = ve.VehAvailCore;
                    if (veCore.Status == "Available")
                    {
                        StdVehicleExtend std = new StdVehicleExtend()
                        {
                            supplierInfo = supplierInfo,
                            veCore = veCore,
                            availRateRQ = availRateRQ
                        };
                        //门店信息

                        std.Supplier = EnumCarSupplier.ABG;
                        std.Vendor = EnumHelper.GetEnumTypeByStr<EnumCarVendor>(supplierInfo.Vendor);
                        std.VendorName = supplierInfo.Vendor;

                        std.VehicleCode = veCore.Vehicle?.VehMakeModel?.Code;
                        std.ProductCode = $"{EnumCarSupplier.ABG}_{loc.FirstOrDefault().Code}_{std.VehicleCode}";

                        std.VehicleName = veCore.Vehicle?.VehMakeModel?.Name.ToString();
                        if (veCore.Vehicle.VehType.DoorCount > 0)
                        {
                            std.DoorCount = (EnumCarDoorCount)veCore.Vehicle.VehType.DoorCount;
                        }
                        switch (veCore.Vehicle.VehType.DoorCount)
                        {
                            case 2: std.DoorCount = EnumCarDoorCount.Door2; break;
                            case 3: std.DoorCount = EnumCarDoorCount.Door3; break;
                            case 4: std.DoorCount = EnumCarDoorCount.Door4; break;
                            case 5: std.DoorCount = EnumCarDoorCount.Door5; break;
                            default:
                                break;
                        }
                        //车型组
                        std.VehicleGroup = (EnumCarVehicleGroup)SIPPHelper.SIPPCodeAnalysis(std.VehicleCode, 3);
                        //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、
                        //8（旅行车）或 9（皮卡）
                        std.VehicleCategory = BuildCarType(veCore.Vehicle.VehType.VehicleCategory.ToString());
                        // 尺寸必须等于以下产品之一：1（小型）、2（小型型）、3（经济型）、4（紧凑型）、
                        //  5（中型）、6（中级）、7（标准）、8（全尺寸）、9（豪华）、10（高级）或11（小型货车）。
                        std.VehicleClass = (EnumCarVehicleClass)veCore.Vehicle.VehClass.Size;
                        std.AirConditioning = veCore.Vehicle.AirConditionInd;

                        std.PictureURL = "https://www.avis.com/car-rental/images/global/en/rentersguide/vehicle_guide/" + veCore.Vehicle.PictureURL;
                        std.TransmissionType = veCore.Vehicle.TransmissionType == "Manual" ? EnumCarTransmissionType.Manual : EnumCarTransmissionType.Automatic;

                        var rentalRate = veCore.RentalRate;
                        var rateDistance = rentalRate.RateDistance;
                        //里程限制
                        StdRateDistance rateinfo = new StdRateDistance
                        {
                            Unlimited = rateDistance.Unlimited,
                            DistUnitName = rateDistance.DistUnitName == "Km" ? EnumCarDistUnitName.Km : EnumCarDistUnitName.Mile,
                            Quantity = rateDistance.Quantity,
                            //VehiclePeriodUnitName 只有这三种“Rental Period”, “Day” or “Hour”
                            VehiclePeriodUnitName = rateDistance.VehiclePeriodUnitName == "Day" ? EnumCarPeriodUnitName.Day : rateDistance.VehiclePeriodUnitName == "Hour" ? EnumCarPeriodUnitName.Hour : EnumCarPeriodUnitName.RentalPeriod,
                        };
                        //Purpose枚举值 目的设置为：1租车2单程费用5升级5机场/城市/其他附加费5机场特许费5年龄附加费
                        //6额外距离收费6车辆登记费6以色列印花税7地方税7奥地利合同税7货物和服务税
                        //8额外里程/额外距离收费9额外10额外一周10天11小时13年龄附加费22预支付费用
                        //28可选费用
                        if (rentalRate.RateDistance.Unlimited == false)
                        {
                            var charges = rentalRate.VehicleCharges.VehicleCharge.Where(n => n.Purpose == "8").ToList();
                            if (charges.Count > 0)
                            {
                                rateinfo.Currency = charges.FirstOrDefault().CurrencyCode;
                                rateinfo.Amount = charges.FirstOrDefault().Amount;
                                rateinfo.Description = charges.FirstOrDefault().Description;
                            }
                        }
                        std.RateDistance = rateinfo;
                        //价格
                        var totalCharge = veCore.TotalCharge;

                        //其他费用  强制要服务，又不包含在总价中
                        List<StdCurrencyAmount> stdAmount = new List<StdCurrencyAmount>();
                        //押金
                        StdCurrencyAmount deposit = new StdCurrencyAmount
                        {
                            PayWhen = EnumCarPayWhen.NoNeed,
                            Type = EnumCarCoverageType.Deposit,
                            Desc = "300-5000USD",
                            Currency = "USD",
                            Amount = 300
                        };
                        stdAmount.Add(deposit);
                        //燃油政策 所有都为FullToFull
                        std.FuelPolicy = EnumCarFuelPolicy.FullToFull;
                        //费率
                        veCore.Fees?.ToList().ForEach(f =>
                        {
                            std.Fees.Add(new StdFee()
                            {
                                Description = f.Description,
                                CurrencyCode = f.CurrencyCode,
                                Amount = f.Amount,
                                TaxInclusive = !(f.TaxInclusive == false),
                                IncludedInRate = !(f.IncludedInRate == false),
                                IncludedInEstTotalInd = !(f.IncludedInEstTotalInd == false)
                            });
                        });
                        //车辆收费
                        List<StdVehicleCharge> vehCharges = new List<StdVehicleCharge>();
                        var vehChargesList = rentalRate.VehicleCharges.VehicleCharge;

                        //车辆收费
                        var vehicleChargesList = vehChargesList.ToList();
                        foreach (var item in vehicleChargesList)
                        {
                            var no_included = item.IncludedInRate == false && item.Amount > 0;//不包含到总价
                            var purpose = EnumCarCoverageType.None;
                            if (item.Purpose == "1")
                            {
                                purpose = EnumCarCoverageType.VehicleVental;
                            }
                            if (item.Purpose == "2")
                            {
                                purpose = EnumCarCoverageType.OneWayFee;
                            }
                            if (item.Purpose == "5")
                            {
                                purpose = EnumCarCoverageType.VehicleVental;
                            }
                            if (item.Purpose == "7")
                            {
                                purpose = EnumCarCoverageType.Tax;
                            }
                            if (item.Purpose == "13")
                            {
                                purpose = EnumCarCoverageType.YoungDriverSurcharge;
                            }
                            if (item.Description == "Additional Distance Charge")
                            {
                                purpose = EnumCarCoverageType.AdditionalDistance;
                            }
                            if (item.Description == "Additional Mileage")
                            {
                                purpose = EnumCarCoverageType.LimitedMileageInformation;
                            }

                            StdVehicleCharge newItem = new StdVehicleCharge
                            {
                                Purpose = purpose,
                                PurposeDescription = item.Description,
                                Description = item.Description,
                                CurrencyCode = item.CurrencyCode,
                                Amount = item.Amount,
                                TaxInclusive = !(item.TaxInclusive == false),
                                IncludedInEstTotalInd = !(item.IncludedInRate == false)
                            };
                            vehCharges.Add(newItem);
                        }
                        std.VehicleCharges = vehCharges;
                        //需要从规则取
                        if (!string.IsNullOrWhiteSpace(veCore.Vehicle.DriveType))
                        {
                            Log.Information($"usertodel:存在驱动枚举");
                        }
                        switch (veCore.Vehicle.DriveType)
                        {
                            case "AWD":
                                std.DriveType = EnumCarDriveType.AWD;
                                break;

                            case "4WD":
                                std.DriveType = EnumCarDriveType.WD4;
                                break;

                            case "Unspecified":
                                std.DriveType = EnumCarDriveType.Unspecified;
                                break;

                            default:
                                break;
                        }
                        if (!string.IsNullOrWhiteSpace(veCore.Vehicle.FuelType))
                        {
                            Log.Information($"todel:燃料类型{veCore.Vehicle.FuelType}");

                            std.FuelType = EnumHelper.GetEnumTypeByStr<EnumCarFuelType>(veCore.Vehicle.FuelType);
                        }
                        if (!string.IsNullOrWhiteSpace(veCore.Vehicle.PassengerQuantity))
                        {
                            Log.Information($"todel:乘客数量{veCore.Vehicle.PassengerQuantity}");
                            std.PassengerQuantity = Convert.ToInt32(veCore.Vehicle.PassengerQuantity);
                        }
                        if (veCore.Vehicle.BaggageQuantity > 0)
                        {
                            Log.Information($"todel:行李数量{veCore.Vehicle.BaggageQuantity}");
                            std.PassengerQuantity = veCore.Vehicle.BaggageQuantity;
                        }

                        //保险
                        List<StdPricedCoverage> stdPricedCoverages = new List<StdPricedCoverage>();
                        var veinfo = ve.VehAvailInfo;
                        if (veinfo != null)
                        {
                            foreach (var pricedCoverage in veinfo.PricedCoverages?.PricedCoverageList)
                            {
                                var charge = pricedCoverage.Charge;
                                var no_included = charge.IncludedInRate == false && charge.Amount > 0;//不包含到总价

                                //todel
                                if (charge.IncludedInEstTotalInd == true)
                                {
                                    Log.Information($"todel:【{charge.IncludedInEstTotalInd}】");
                                }
                                StdPricedCoverage stdPriced = new StdPricedCoverage()
                                {
                                    Required = pricedCoverage.Required,
                                    CoverageType = BuildEnumCarCoverageType(pricedCoverage.Coverage.CoverageType),
                                    CoverageDescription = pricedCoverage.Coverage.CoverageDetails?.Description,
                                    Description = pricedCoverage.Coverage.CoverageDetails?.Description,
                                    CurrencyCode = charge.CurrencyCode,
                                    Amount = charge.Amount,
                                    TaxInclusive = charge.TaxInclusive,
                                    IncludedInRate = charge.IncludedInRate,
                                    IncludedInEstTotalInd = charge.IncludedInEstTotalInd,

                                    MaxCharge = charge.MinMax?.MaxCharge,
                                    MinCharge = charge.MinMax?.MinCharge,
                                };
                                if (charge.Calculation != null)
                                {
                                    stdPriced.Calculation = new StdCalculation
                                    {
                                        Quantity = charge.Calculation.Quantity,
                                        UnitName = charge.Calculation.UnitName,
                                        UnitCharge = charge.Calculation.UnitCharge
                                    };
                                }
                                //CDW  //包含才输出（起赔额包括0：全险
                                if (pricedCoverage.Coverage.CoverageType == 7 && !no_included && charge.Amount >= 0)
                                {
                                    StdCurrencyAmount ocitem = new StdCurrencyAmount
                                    {
                                        Type = EnumCarCoverageType.CollisionDamageWaiver,
                                        PayWhen = EnumCarPayWhen.NoNeed,
                                        Desc = pricedCoverage.Coverage.CoverageDetails?.Description,
                                        Amount = pricedCoverage.Charge.Amount,
                                        Currency = pricedCoverage.Charge.CurrencyCode
                                    };
                                    stdAmount.Add(ocitem);
                                }    //TP保险 包含才输出（起赔额包括0：全险
                                else if (pricedCoverage.Coverage.CoverageType == 48 && !no_included && charge.Amount >= 0)
                                {
                                    StdCurrencyAmount ocitem = new StdCurrencyAmount
                                    {
                                        Type = EnumCarCoverageType.TheftProtection,
                                        PayWhen = EnumCarPayWhen.NoNeed,
                                        Desc = pricedCoverage.Coverage.CoverageDetails?.Description,
                                        Amount = pricedCoverage.Charge.Amount,
                                        Currency = pricedCoverage.Charge.CurrencyCode
                                    };
                                    stdAmount.Add(ocitem);
                                }
                                /* else
                                 {
                                     //其他费用
                                     if (no_included)
                                     {
                                         StdCurrencyAmount ocitem = new StdCurrencyAmount
                                         {
                                             Type = EnumCarCoverageType.TheftProtection,
                                             PayWhen = EnumCarPayWhen.NoNeed,
                                             Desc = ("PricedCoverages" + pricedCoverage.Coverage.CoverageType + "-" + pricedCoverage.Coverage.CoverageDetails?.Description),
                                             Amount = pricedCoverage.Charge.Amount,
                                             Currency = pricedCoverage.Charge.CurrencyCode
                                         };
                                         stdAmount.Add(ocitem);
                                     }
                                 }*/
                                stdPricedCoverages.Add(stdPriced);
                            }
                        }
                        std.PricedCoverages = stdPricedCoverages;
                        //价格
                        std.TotalCharge = new StdTotalCharge
                        {
                            //支付模式根据供应商的来
                            //PayType = supplierInfo.PayType, //先不赋值 交给后面多种支付方式赋值
                            PriceType = EnumCarPriceType.NetRate, //底价模式
                            Currency = totalCharge.CurrencyCode,
                            TotalAmount = totalCharge.EstimatedTotalAmount,
                            RentalAmount = totalCharge.RateTotalAmount,
                            OtherAmounts = stdAmount
                        };
                        var pickUp = dto.startLoc;
                        var endLoc = dto.endLoc;
                        std.Location = new StdLocation()
                        {
                            PickUp = new StdLocationInfo()
                            {
                                SuppLocId = pickUp.SuppLocId,
                                VendorLocId = pickUp.VendorLocId,
                                LocationName = pickUp.LocationName,
                                Latitude = pickUp.Latitude,
                                Longitude = pickUp.Longitude,
                                LocationId = pickUp.LocationId,
                                CityId = pickUp.CityId,
                                PostalCode = pickUp.PostalCode,
                                StateCode = pickUp.StateCode,
                                CityName = pickUp.CityName,
                                CountryCode = pickUp.CountryCode,
                                CloseTime = pickUp.CloseTime,
                                OpenTime = pickUp.OpenTime,
                                CountryName = pickUp.CountryName,
                            },
                            DropOff = new StdLocationInfo()
                            {
                                SuppLocId = endLoc.SuppLocId,
                                VendorLocId = endLoc.VendorLocId,
                                LocationName = endLoc.LocationName,
                                Latitude = endLoc.Latitude,
                                Longitude = endLoc.Longitude,
                                LocationId = endLoc.LocationId,
                                CityId = endLoc.CityId,
                                PostalCode = endLoc.PostalCode,
                                StateCode = endLoc.StateCode,
                                CityName = endLoc.CityName,
                                CountryCode = endLoc.CountryCode,
                                CloseTime = endLoc.CloseTime,
                                OpenTime = endLoc.OpenTime,
                                CountryName = endLoc.CountryName,
                            }
                        };
                        if (pickUp.OperationTime != null)
                        {
                            std.Location.PickUp.OperationTimes = JsonConvert.DeserializeObject<List<StdOperationTime>>(pickUp.OperationTime);
                        }
                        if (endLoc.OperationTime != null)
                        {
                            std.Location.DropOff.OperationTimes = JsonConvert.DeserializeObject<List<StdOperationTime>>(endLoc.OperationTime);
                        }
                        var driverInfo = driverList.FirstOrDefault(n => n.Code == loc.FirstOrDefault()?.Code && n.CarGroup == std.VehicleCode[0].ToString());

                        if (driverInfo != null)
                        {
                            std.MinDriverAge = driverInfo.MinimumAge.HasValue ? driverInfo.MinimumAge.Value : 0;
                            std.MaxDriverAge = driverInfo.MaximumAge.HasValue ? driverInfo.MaximumAge.Value : 0;
                        }
                        //取车前免费取消  noshow 取 60 EUR
                        std.CancelPolicy = new StdCancelPolicy
                        {
                            CancelType = EnumCarCancelType.FeeCancel,
                            Rules = new List<StdCancelRule>
                            {
                                new StdCancelRule
                                {
                                    DeductType = EnumCarDeductType.Free,
                                    Currency =  totalCharge.CurrencyCode,
                                    StartTime = DateTime.Now,
                                    EndTime = availRateRQ.VehAvailRQCore.VehRentalCore.PickUpDateTime,
                                    DeductValue=0,
                                }
                            }
                        };

                        //构建ratecode这里下单需要解析出来
                        var rateCode = $"UserVerdorType_{rentCore.PickUpLocation.LocationCode}_{rentCore.ReturnLocation.LocationCode}";
                        rateCode = rateCode + $"_{veCore.Vehicle.VehType.VehicleCategory}_{veCore.Vehicle.VehClass.Size}";
                        rateCode = rateCode + $"_{std.VehicleCode}_{veCore.RentalRate.RateQualifier.RateCategory}";
                        rateCode = rateCode + $"_{veCore.RentalRate.RateQualifier.RateQualifierValue}";

                        std.RateCode = rateCode;
                        //result.Add(std);
                        //根据不同的支付类型生成两条记录
                        foreach (var supplier in supplierInfo.secSuppliers)
                        {
                            var newItem = _mapper.Map<StdVehicleExtend, StdVehicleExtend>(std);
                            newItem.TotalCharge = new StdTotalCharge
                            {
                                //支付模式根据供应商的来
                                //PayType = supplierInfo.PayType, //先不赋值 交给后面多种支付方式赋值
                                PriceType = std.TotalCharge.PriceType, //底价模式
                                Currency = std.TotalCharge.Currency,
                                TotalAmount = std.TotalCharge.TotalAmount,
                                RentalAmount = std.TotalCharge.RentalAmount,
                                OtherAmounts = std.TotalCharge.OtherAmounts,
                                PayType = supplier.PayType
                            };
                            newItem.RateCode = newItem.RateCode.Replace("UserVerdorType", supplier.VerdorType.ToString());
                            result.Add(newItem);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 请求rateRule规则 构建 TermsAndConditions实体
        /// </summary>
        /// <returns></returns>
        public async Task BuildTermsAndConditions(StdVehicleExtend std)
        {
            try
            {
                var rateRule = await GetRateRule(std.availRateRQ, std.veCore, std.supplierInfo);
                if (rateRule != null)
                {
                    List<StdTermAndCondition> termsAndConditions = new List<StdTermAndCondition>();
                    if (rateRule.Errors != null && rateRule.Errors.ErrorList.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(rateRule);
                        Log.Error($"获取Rule失败{string.Join(",", rateRule.Errors.ErrorList.Select(n => n.Message))}");
                    }
                    if (rateRule.Warnings != null && rateRule.Warnings.WarningList.Count > 0)
                    {
                        Log.Information($"usertodel$存在变价场景待处理{JsonConvert.SerializeObject(rateRule)}");
                        //判断是否变价  Requested rate has changed
                    }
                    //装备
                    var equips = rateRule.PricedEquips;
                    List<StdPricedEquip> eqList = new List<StdPricedEquip>();
                    foreach (var eq in equips)
                    {
                        StdPricedEquip stdPricedEquip = new StdPricedEquip();
                        stdPricedEquip.EquipType = BuildPricedEquip(eq.Equipment.EquipType);
                        if (stdPricedEquip.EquipType == EnumCarEquipType.None)
                        {
                            //Log.Error($"存在没有处理的设备{JsonConvert.SerializeObject(eq)}");
                            continue;
                        }
                        if (stdPricedEquip.EquipType == EnumCarEquipType.AdditionalDriver)
                        {
                            //Log.Information($"存在超龄驾驶员");
                        }
                        stdPricedEquip.EquipDescription = "";
                        stdPricedEquip.Unit = BuildEquipUnitType(eq.Charge.Calculation.UnitName);
                        stdPricedEquip.Currency = eq.Charge.CurrencyCode;
                        stdPricedEquip.UnitPrice = eq.Charge.Calculation.UnitCharge;
                        stdPricedEquip.TaxInclusive = eq.Charge.TaxInclusive;
                        stdPricedEquip.IncludedInEstTotalInd = eq.Charge.IncludedInEstTotalInd;
                        stdPricedEquip.MaxQuantity = 1;
                        eqList.Add(stdPricedEquip);
                    }
                    std.PricedEquips = eqList;
                    //政策信息
                    //db政策  信用卡政策  年轻驾驶员要求
                    /*      var cardPolicy = await _cardRepository.GetByIdAsync("select * from ABG_CreditCardPolicy where AvisLocationCode=@Location and VehicleSIPPCode=@SIPPCode", new { Location = loc.FirstOrDefault().Code, SIPPCode = std.VehicleCode });
                          if (cardPolicy != null)
                          {
                              StdTermAndCondition cardConfition = new StdTermAndCondition
                              {
                                  Titel = "Credit Card Requirements",
                                  Sections = new List<StdSection>
                                  {
                                      new StdSection{Text=$"Number of Credit Cards Required for rental is {cardPolicy.NumCreditCardsRequired}"}
                                  }
                              };
                              //信用卡要求个数 先不添加政策
                              //termsAndConditions.Add(cardConfition);
                          }
    */
                    var vendorMessages = rateRule.VendorMessages;

                    foreach (var msg in vendorMessages)
                    {
                        List<StdSection> stdSections = new List<StdSection>();
                        var title = msg.SubSection;
                        foreach (var content in title)
                        {
                            StdSection stdSection = new StdSection
                            {
                                Title = content.SubTitle,
                                Text = string.Join("<br />", content.Paragraph.ListItem.Text)
                            };
                            stdSections.Add(stdSection);
                        }
                        StdTermAndCondition newitem = new StdTermAndCondition
                        {
                            Code = msg.Title,
                            Sections = stdSections,
                            Titel = msg.Title
                        };
                        termsAndConditions.Add(newitem);
                    }
                    std.TermsAndConditions = termsAndConditions;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"构建政策失败{ex.Message}");
            }
        }

        public async Task GetLocationDetail(SupplierInfo supper, LocationDetails loc)
        {
            try
            {
                ABG_OTA_VehLocSearchRQ postData = new ABG_OTA_VehLocSearchRQ
                {
                    Version = "1.0",
                    POS = new POS
                    {
                        Source = new Source
                        {
                            RequestorID = new RequestorID { ID = supper.DefaultIATA, Type = 5 }
                        }
                    },
                    VehLocSearchCriterion = new VehLocSearchCriterion
                    {
                        Address = new Models.RQs.Address
                        {
                            AddressLine = loc.Address.StreetNmbr,
                            CityName = loc.Address.CityName,
                            PostalCode = loc.Address.PostalCode,
                            County = loc.Address.CountryName?.Name,
                            StateProv = new Models.RQs.StateProv { StateCode = loc.Address.StateProv.StateCode },
                            CountryName = new Models.RQs.CountryName { Code = loc.Address.CountryName.Code }
                        },
                        Radius = new Radius { DistanceMax = 40, DistanceMeasure = "Miles" }
                    },
                    Vendor = new LocVendor
                    {
                        Code = supper.Vendor
                    },
                    TPA_Extensions = new TPA_Extensions
                    {
                        SortOrderType = "DESCENDING",
                        TestLocationType = "No",
                        LocationStatusType = "OPEN",
                        LocationType = "RENTAL"
                    },
                };
                var res = BuildEnvelope(new CommonRequest { ABG_OTA_VehLocSearchRQ = postData, Type = ApiEnum.Location });
                var model = ABGXmlHelper.GetResponse<ABG_OTA_VehAvailRateRS>(res);
            }
            catch (Exception ex)
            {
                Log.Error($"获取地址详情错误{ex.Message}");
            }
        }

        /// <summary>
        /// 构建取车还车地址
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        private StdLocationInfo BuildLocationDetail(LocationDetails loc, ABGLocation db_loc)
        {
            if (db_loc == null)
            {
                Log.Error($"找不到对应的门店信息{JsonConvert.SerializeObject(loc)}");
                return new StdLocationInfo
                {
                    CounterLocation = BuildLocationType(loc), //usertodo
                    LocationName = loc.Name,
                    CountryCode = loc.Address.CountryName.Code,
                    CountryName = loc.Address.CountryName.Name,
                    StateProv = loc.Address.StateProv?.Name,
                    StateCode = loc.Address.StateProv.StateCode,
                    CityName = loc.Address.CityName,
                    PostalCode = loc.Address.PostalCode,
                    Telephone = loc.Telephone?.PhoneNumber,
                    Airport = loc.AtAirport,
                };
            }
            else
            {
                var counterLocation = EnumCarCounterLocation.Other;
                if (db_loc.RentalType == "City/Downtown")
                {
                    counterLocation = EnumCarCounterLocation.CityDowntown;
                }
                else if (db_loc.RentalType == "Airport")
                {
                    counterLocation = EnumCarCounterLocation.Airport;
                }
                else if (db_loc.RentalType == "Railway/Bus Station")
                {
                    counterLocation = EnumCarCounterLocation.TrainStation;
                }
                StdLocationInfo startLoc = new StdLocationInfo
                {
                    CounterLocation = counterLocation, //usertodo
                    LocationName = loc.Name,
                    //LocationId=loc.Code, //locationid没有？
                    CountryCode = db_loc.LocationName,
                    CountryName = loc.Address.CountryName.Name,
                    StateProv = loc.Address.StateProv?.Name,
                    StateCode = loc.Address.StateProv?.StateCode,
                    CityName = db_loc.City,
                    Address = db_loc.Address,
                    PostalCode = loc.Address.PostalCode,
                    Telephone = db_loc.PhoneNumber,
                    Latitude = db_loc.Latitude,
                    Longitude = db_loc.Longitude,
                    Airport = loc.AtAirport,
                    AirportCode = db_loc.APOCode,
                    RailwayStation = counterLocation == EnumCarCounterLocation.TrainStation ? true : false,
                    Email = db_loc.Email,

                    SuppLocId = db_loc.LocationCode,
                    /*VendorLocId = loc.AdditionalInfo.ParkLocation?.Location.ToString(), //停车位置 usertodo
                    SuppLocId = loc.AdditionalInfo.CounterLocation?.Location.ToString(),//柜台位置 usertodo
                    ParkLocation = $"{loc.AdditionalInfo.ParkLocation?.Location.ToString()}"*/
                };
                return startLoc;
            }
        }

        /// <summary>
        /// 构建地址枚举
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        private EnumCarCounterLocation BuildLocationType(LocationDetails loc)
        {
            if (loc.AtAirport)
            {
                return EnumCarCounterLocation.Airport;
            }
            else
            {
                Log.Information($"usertodel:门店地址待处理{loc.Code}");
                return EnumCarCounterLocation.Other;
            }
        }

        /// <summary>
        /// 构建供应商那边设备枚举
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetSupplierEquipType(EnumCarEquipType type)
        {
            foreach (var pair in dict)
            {
                if (EqualityComparer<EnumCarEquipType>.Default.Equals(pair.Value, type))
                {
                    return pair.Key;
                }
            }
            return "";
        }

        /// <summary>
        /// 构建设备单位枚举
        /// </summary>
        /// <param name="unitName"></param>
        /// <returns></returns>
        public EnumCarPeriodUnitName BuildEquipUnitType(string unitName)
        {
            switch (unitName)
            {
                case "Daily":
                    return EnumCarPeriodUnitName.Day;

                case "RentalPeriod":
                    return EnumCarPeriodUnitName.RentalPeriod;

                case "Hour":
                    return EnumCarPeriodUnitName.Hour;
            }
            Log.Information($"存在找不到单位的设备类型{unitName}");
            return EnumCarPeriodUnitName.RentalPeriod;
        }

        private Dictionary<string, EnumCarEquipType> dict = new Dictionary<string, EnumCarEquipType>()
        {
              {"119",EnumCarEquipType.AdditionalDriver },
              {"9",EnumCarEquipType.CBS },
              {"7",EnumCarEquipType.InfantSeat },
              {"8",EnumCarEquipType.CST },
              {"13",EnumCarEquipType.GPS },
              {"10",EnumCarEquipType.SnowChains },
              {"14",EnumCarEquipType.SnowTires },
              {"226",EnumCarEquipType.CBF },
              {"59",EnumCarEquipType.CO2 },
              {"132",EnumCarEquipType.CSI },
              {"2",EnumCarEquipType.BYC },
              {"4",EnumCarEquipType.SKR },
              {"3",EnumCarEquipType.LUG },
              {"55",EnumCarEquipType.WIFI },
              {"131",EnumCarEquipType.BlueTooth },
              {"146",EnumCarEquipType.Mobile_Phone_Charger },
              {"47",EnumCarEquipType.Snow_Board_Rack },
              {"171",EnumCarEquipType.Baby_Stroller },
        };

        /// <summary>
        /// 构建设备类型
        /// </summary>
        public EnumCarEquipType BuildPricedEquip(string? type)
        {
            if (dict.TryGetValue(type, out EnumCarEquipType value))
            {
                return value;
            }
            else
            {
                return EnumCarEquipType.None; //跨国*/
            }
            /*switch (type)
            {
                case 119:
                    return EnumCarEquipType.AdditionalDriver;

                case 9:
                    return EnumCarEquipType.CBS;

                case 7:
                    return EnumCarEquipType.InfantSeat;

                case 147: //路边服务 Curbside Service

                    return EnumCarEquipType.None;

                case 8:
                    return EnumCarEquipType.CST;

                case 157:
                    return EnumCarEquipType.None;

                case 13:
                    return EnumCarEquipType.GPS;

                case 10:
                    return EnumCarEquipType.SnowChains;

                case 14:
                    return EnumCarEquipType.SnowTires;

                case 226:
                    return EnumCarEquipType.CBF;

                case 59:
                    return EnumCarEquipType.CO2;

                case 132:
                    return EnumCarEquipType.CSI;

                case 2:
                    return EnumCarEquipType.BYC;

                case 3:
                    return EnumCarEquipType.LUG;

                case 4:
                    return EnumCarEquipType.SKR;

                case 55:
                    return EnumCarEquipType.WIFI;

                case 131:
                    return EnumCarEquipType.BlueTooth;

                case 146:
                    return EnumCarEquipType.Mobile_Phone_Charger;

                case 47:
                    return EnumCarEquipType.Snow_Board_Rack;

                case 171:
                    return EnumCarEquipType.Baby_Stroller;
            }
            Log.Information($"usertodel:出现没有匹配到的设备类型[{type}]");
            return EnumCarEquipType.None;*/
        }

        /// <summary>
        ///  保险类型转换
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public EnumCarCoverageType BuildEnumCarCoverageType(int coverageType)
        {
            switch (coverageType)
            {
                case 1:
                    return EnumCarCoverageType.AdditionalDistance;

                case 7:
                    return EnumCarCoverageType.CollisionDamageWaiver;

                case 32:
                    return EnumCarCoverageType.PersonalAccidentInsurance;

                case 35: //Additional Protection Insurance   Personal Effects Protection 有两种怎么赋值
                    return EnumCarCoverageType.PersonalEffectsCoverage;

                case 40:
                    return EnumCarCoverageType.SuperCollisionDamageWaiver;

                case 44: //Super Personal Accident Insurance  //usertodo
                    return EnumCarCoverageType.SuperCover;

                case 45: //Super Theft Protection 赋值为tp?
                    return EnumCarCoverageType.TheftProtection;

                case 48:
                    return EnumCarCoverageType.TheftProtection;

                case 61: //Package Protection
                    return EnumCarCoverageType.None;

                case 63:
                    return EnumCarCoverageType.ThirdPartyLiability;

                default:
                    Log.Error($"usertodel:{coverageType}");
                    return EnumCarCoverageType.None;
            }
        }

        /// <summary>
        /// 获取费率规则
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="veCore"></param>
        /// <returns></returns>
        public async Task<ABG_OTA_VehRateRuleRS> GetRateRule(ABG_OTA_VehAvailRateRQ availRateRQ, VehAvailCore veCore, SupplierInfo supplierInfo)
        {
            var ruleRq = new ABG_OTA_VehRateRuleRQ
            {
                //返回请求数
                Version = "1.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID { ID = supplierInfo.DefaultIATA, Type = 5 }
                    }
                },
                RentalInfo = new RentalInfo
                {
                    VehRentalCore = availRateRQ.VehAvailRQCore.VehRentalCore,
                    //这个信息 必填 首选 Preferred
                    VehicleInfo = new VehicleInfo
                    {
                        AirConditionPref = "Preferred", //指空调偏好 有空调信息返回 这里不知道要不要处理
                        ClassPref = "Preferred",  //指车辆等级偏好
                        TransmissionPref = "Preferred", //指变速箱偏好。
                        TransmissionType = veCore.Vehicle.TransmissionType, //指变速箱类型。
                        TypePref = "Preferred", //指车辆类型偏好
                        VehType = new Models.RQs.VehType { VehicleCategory = veCore.Vehicle.VehType.VehicleCategory },  //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、8（旅行车）或 9（皮卡）。
                        VehClass = new VehClass { Size = veCore.Vehicle?.VehClass?.Size == null ? 0 : veCore.Vehicle.VehClass.Size }, //当 VehicleCategory 为 “Car(1)” 时，DoorCount 必须是 “2” 或 “4”。 其他值可忽略
                        VehGroup = new VehGroup { GroupType = "SIPP", GroupValue = veCore.Vehicle?.VehMakeModel?.Code }
                    },
                    RateQualifier = veCore.RentalRate.RateQualifier,
                    CustomerID = new CustomerID { ID = availRateRQ.VehAvailRQInfo.Customer.Primary.CitizenCountryName.Code, Type = "1" }
                },
            };
            var res = BuildEnvelope(new CommonRequest { OTA_VehRateRuleRQ = ruleRq, Type = ApiEnum.Rule });
            var model = ABGXmlHelper.GetResponse<ABG_OTA_VehRateRuleRS>(res);

            return model;
        }

        public string BuildEnvelope(CommonRequest model)
        {
            ns_Request ns_Request = new ns_Request();
            switch (model.Type)
            {
                case ApiEnum.List:
                    ns_Request.OTA_VehAvailRateRQ = model.OTA_VehAvailRateRQ;
                    break;

                case ApiEnum.Rule:
                    ns_Request.OTA_VehRateRuleRQ = model.OTA_VehRateRuleRQ;
                    break;

                case ApiEnum.Create:
                    ns_Request.ABG_OTA_VehResRQ = model.ABG_OTA_VehResRQ;
                    break;

                case ApiEnum.Cancel:
                    ns_Request.ABG_OTAVehCancelRQ = model.ABG_OTAVehCancelRQ;
                    break;

                case ApiEnum.Detail:
                    ns_Request.ABG_OTA_VehRetResRQ = model.ABG_OTA_VehRetResRQ;
                    break;

                case ApiEnum.Location:
                    ns_Request.ABG_OTA_VehLocSearchRQ = model.ABG_OTA_VehLocSearchRQ;
                    break;
            }
            var envelope = new Envelope
            {
                Header = new SOAP_ENV_Header
                {
                    Credentials = new Credentials
                    {
                        UserID = new EncodedString { Value = _setting.UserID, EncodingType = "xsd:string" },
                        Password = new EncodedString { Value = _setting.Password, EncodingType = "xsd:string" }
                    },
                    WSBangRoadmap = new WSBang_Roadmap()
                },
                Body = new SOAP_ENV_Body
                {
                    Request = ns_Request
                }
            };
            return ABGXmlHelper.PostRequest(_setting.Url, envelope, model.Type);
        }

        /// <summary>
        /// 构建车辆类型
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public EnumCarVehicleCategory BuildCarType(string category)
        {
            try
            {
                return (EnumCarVehicleCategory)Convert.ToInt32(category);
            }
            catch (Exception ex)
            {
                Log.Error($"遇到其他车型{category}");
                return EnumCarVehicleCategory.None;
            }
        }

        public async Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ originModel, SecSupplier supplier, ABG_OTA_VehResRQ createOrderRQ, int timeout = 15000)
        {
            StdCreateOrderRS result = new StdCreateOrderRS();
            var res = BuildEnvelope(new CommonRequest { ABG_OTA_VehResRQ = createOrderRQ, Type = ApiEnum.Create });

            var spmodel = ABGXmlHelper.GetResponse<ABG_OTAVehResRS>(res);
            if (spmodel.Errors != null && spmodel.Errors.ErrorList.Count > 0)
            {
                var msg = string.Join(",", spmodel.Errors.ErrorList.Select(n => n.Message));
                Log.Error($"下单失败");
                result.OrderSuc = false;
                result.Message = msg;
                return result;
            }
            var veCore = spmodel.VehResRSCore.VehReservation.VehSegmentCore;
            var loc = spmodel.VehResRSCore.VehReservation.VehSegmentInfo.LocationDetails;
            var customer = spmodel.VehResRSCore.VehReservation.Customer;
            ABGCarProReservation order = new ABGCarProReservation
            {
                OrderNo = originModel.OrderNo,
                ReservationId = veCore.ConfID.ID,
                ReservationType = veCore.ConfID.Type.ToString(),
                PickUpDateTime = veCore.VehRentalCore.PickUpDateTime,
                ReturnDateTime = veCore.VehRentalCore.ReturnDateTime,
                PickUpLocationCode = veCore.VehRentalCore.PickUpLocation.LocationCode,
                ReturnLocationCode = veCore.VehRentalCore.ReturnLocation.LocationCode,
                CodeContext = veCore.VehRentalCore.PickUpLocation.CodeContext,
                SIPP = veCore.Vehicle.VehMakeModel.Code,
                TransmissionType = veCore.Vehicle.TransmissionType,
                AirConditionInd = veCore.Vehicle.AirConditionInd.ToString(),
                DriveType = veCore.Vehicle.DriveType,
                PassengerQuantity = veCore.Vehicle.PassengerQuantity?.ToString(),
                BaggageQuantity = veCore.Vehicle.BaggageQuantity.ToString(),
                FuelType = veCore.Vehicle.FuelType,
                VehicleCategory = veCore.Vehicle.VehType.VehicleCategory.ToString(),
                DoorCount = veCore.Vehicle.VehType.DoorCount.ToString(),
                VehClass = veCore.Vehicle.VehClass.Size.ToString(),
                CarName = veCore.Vehicle.VehMakeModel.Name,
                PictureURL = veCore.Vehicle.PictureURL,
                RateDistanceInfo = JsonConvert.SerializeObject(veCore.RentalRate.RateDistance),
                RateTotalAmount = veCore.TotalCharge.RateTotalAmount,
                EstimatedTotalAmount = veCore.TotalCharge.EstimatedTotalAmount,
                CurrencyCode = veCore.TotalCharge.CurrencyCode,
                Location_Code = loc.LastOrDefault().Code,
                ReturnLocation_Code = loc.FirstOrDefault().Code,
                GivenName = originModel.FirstName,
                Surname = originModel.LastName,
                Email = originModel.Email,
                Telephone = originModel.ContactNumber,
                VendorName = supplier.Vendor,
                VendorCode = supplier.IATA,
                //AddressInfo = JsonConvert.SerializeObject(customer.Primary.Address),
                //DocInfo = JsonConvert.SerializeObject(customer.Primary.Document),
                CreateTime = DateTime.Now,
                OrderStatus = "Comfirmed", //usertodo
                PayType = supplier.PayType.ToString(),
                RateCode = originModel.RateCode
            };
            await _proOrdRepository.InsertAsync(order);
            result.OrderSuc = true;
            result.SuppOrderId = spmodel.VehResRSCore.VehReservation.VehSegmentCore.ConfID.ID;
            /*          result.SuppOrderStatus = "";
                      result.SuppConfirmNumber =*/
            result.SuppCurrency = spmodel.VehResRSCore.VehReservation.VehSegmentCore.TotalCharge.CurrencyCode;
            result.SuppTotalPrice = spmodel.VehResRSCore.VehReservation.VehSegmentCore.TotalCharge.EstimatedTotalAmount;
            result.SuppConfirmed = true;
            return result;
        }

        public async Task<StdCancelOrderRS> CancelOrderAsync(ABGCarProReservation order, ABG_OTAVehCancelRQ cancelOrderRQ, int timeout = 15000)
        {
            StdCancelOrderRS result = new StdCancelOrderRS();
            var res = BuildEnvelope(new CommonRequest { ABG_OTAVehCancelRQ = cancelOrderRQ, Type = ApiEnum.Cancel });
            var spModel = ABGXmlHelper.GetResponse<ABG_OTA_VehCancelRS>(res);
            if (spModel == null)
            {
                throw new Exception($"解析实体错误{res}");
            }
            if (spModel.Errors != null && spModel.Errors.ErrorList.Count > 0)
            {
                Log.Error($"取消订单出错{string.Join(",", spModel.Errors.ErrorList.Select(n => n.Message))}");
                result.CancelSuc = false;
                result.Message = string.Join(",", spModel.Errors.ErrorList.Select(n => n.Message));
                return result;
            }

            if (spModel.VehCancelRSCore.CancelStatus == "Cancelled")
            {
                result.Currency = order.CurrencyCode;

                result.CancelSuc = true;
                await _proOrdRepository.UpdateBySqlAsync("update ABG_CarProReservation set CancelTime=@CancelTime,OrderStatus=@OrderStatus " +
                "where orderno = @orderno", new { CancelTime = DateTime.Now, OrderStatus = "cancelled", orderno = order.OrderNo });
            }
            else
            {
                result.CancelSuc = false;
            }
            return result;
        }

        public async Task<StdQueryOrderRS> QueryOrderAsync(ABGCarProReservation order, ABG_OTA_VehRetResRQ postdata, int timeout = 1500)
        {
            StdQueryOrderRS result = new StdQueryOrderRS();
            var res = BuildEnvelope(new CommonRequest { ABG_OTA_VehRetResRQ = postdata, Type = ApiEnum.Detail });
            var spModel = ABGXmlHelper.GetResponse<ABG_OTA_VehRetResRS>(res);
            if (spModel == null)
            {
                throw new Exception($"解析实体错误{res}");
            }

            if (spModel.Errors != null && spModel.Errors.ErrorList.Count > 0)
            {
                Log.Error($"查询订单出错{string.Join(",", spModel.Errors.ErrorList.Select(n => n.Message))}");
                if (spModel.Errors.ErrorList.Exists(n => n.ShortText.Contains("31005")))
                {
                    result.IsSuccess = true;
                    result.SuppOrderStatus = "cancelled";
                    result.SuppOrderId = order.ReservationId;
                    result.Currency = order.CurrencyCode;
                    result.Amount = order.EstimatedTotalAmount.Value;
                    result.SIPP = order.SIPP;
                    result.OrigData = res;
                    result.Status = EnumCarReservationStatus.Cancelled;
                }
                else
                {
                    result.IsSuccess = false;
                }
                result.ErrorMessage = string.Join(",", spModel.Errors.ErrorList.Select(n => n.Message));
                return result;
            }
            result.IsSuccess = true;
            result.Status = EnumCarReservationStatus.Confirmed;
            result.SuppOrderId = spModel.VehRetResRSCore.VehReservation.VehSegmentCore.ConfID.ID;
            result.Currency = spModel.VehRetResRSCore.VehReservation.VehSegmentCore.TotalCharge.CurrencyCode;
            result.Amount = spModel.VehRetResRSCore.VehReservation.VehSegmentCore.TotalCharge.EstimatedTotalAmount;
            result.SIPP = spModel.VehRetResRSCore.VehReservation.VehSegmentCore?.Vehicle?.VehMakeModel?.Code;
            result.OrigData = res;
            return result;
        }

        #endregion 原始接口

        #region 业务接口

        /// <summary>
        /// 开多个线程 最多5个
        /// </summary>
        private SemaphoreSlim semaphore = new SemaphoreSlim(10);

        private async Task<List<StdVehicleExtend>> GetVehiclesByloc(QueryDto dto)
        {
            List<StdVehicleExtend> result = new List<StdVehicleExtend>();
            try
            {
                await semaphore.WaitAsync();
                var availRateRQ = new ABG_OTA_VehAvailRateRQ
                {
                    //返回请求数
                    MaxResponses = 10000,
                    ReqRespVersion = "medium",
                    Version = "1.0",
                    POS = new POS
                    {
                        Source = new Source
                        {
                            RequestorID = new RequestorID { ID = dto.SupplierInfo.DefaultIATA, Type = 5 }
                        }
                    },
                    VehAvailRQCore = new VehAvailRQCore
                    {
                        Status = "Available",
                        VehRentalCore = new VehRentalCore
                        {
                            //取车日期 还车日期
                            PickUpDateTime = Convert.ToDateTime(dto.PickUpDateTime),
                            ReturnDateTime = Convert.ToDateTime(dto.ReturnDateTime),
                            //出发地 目的地方
                            PickUpLocation = new Location { LocationCode = dto.PickUpLocationCode },
                            ReturnLocation = new Location { LocationCode = dto.ReturnLocationCode }
                        },
                        VendorPrefs = new VendorPrefs
                        {
                            VendorPref = new VendorPref { CompanyShortName = dto.SupplierInfo.Vendor }
                        },
                        //这个信息 必填 首选 Preferred
                        VehPrefs = new VehPrefs
                        {
                            VehPref = new VehPref
                            {
                                AirConditionPref = "Preferred", //指空调偏好
                                ClassPref = "Preferred",  //指车辆等级偏好
                                TransmissionPref = "Preferred", //指变速箱偏好。
                                TransmissionType = "Automatic", //指变速箱类型。
                                TypePref = "Preferred", //指车辆类型偏好
                                VehType = new VehType { VehicleCategory = 1 },  //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、8（旅行车）或 9（皮卡）。
                                VehClass = new VehClass { Size = 3 } //传经济类型？
                            }
                        },
                        //年龄
                        DriverType = new DriverType { Age = dto.DriverAge },
                        //RateCategory is used to determine if Business rates should be considered.If the RateCategory is
                        //included and equal to 2(Business) or 6(All),business rates will be included.Otherwise only
                        //Leisure rates will be addressed.
                        RateQualifier = new RateQualifier { RateCategory = "6" }
                    },
                    VehAvailRQInfo = new VehAvailRQInfo
                    {
                        Customer = new Customer
                        {
                            //出生国家
                            Primary = new Primary
                            {
                                CitizenCountryName = new CitizenCountryName { Code = dto.CitizenCountryCode }
                            }
                        }
                    }
                };
                var rqList = await VehAvailRate(availRateRQ, dto);
                result.AddRange(rqList);
            }
            catch (Exception ex)
            {
                Log.Error($"请求接口报错{ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
            return result;
        }

        /// <summary>
        /// 查询车辆集合。该供应商支持：目前只发现 IATA
        /// </summary>
        /// <param name="vehicleRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            Log.Information($"接收到参数{JsonConvert.SerializeObject(vehicleRQ)}");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<StdVehicle> res = new List<StdVehicle>();
            try
            {
                var dbModel = new ABGRateCache();
                var cache_key = "";
                var md5Key = "";
                //根据配置确定有没有缓存
                if (_setting.PassMin > 0)
                {
                    cache_key = $"{vehicleRQ.PickUpLocationCode}_{vehicleRQ.PickUpDateTime.ToString("yyyy-MM-ddTHH:mm:ss")}_{vehicleRQ.ReturnLocationCode}_{vehicleRQ.ReturnDateTime.ToString("yyyy-MM-ddTHH:mm:ss")}";//"HK_2024-04-28T10:00:00_BKK_2024-05-01T10:00:00_BKK_30"
                    md5Key = Md5Helper.ComputeMD5Hash(cache_key);
                    dbModel = await _rateCacheRepository.GetByIdAsync("select * from ABG_RateCache where SearchMD5 = @SearchMD5", new { SearchMD5 = md5Key });
                    if (dbModel != null && dbModel.ExpireTime > DateTime.Now)
                    {
                        var cache = GZipHelper.DecompressString(dbModel.RateCache);
                        //修改缓存
                        await _rateCacheRepository.UpdateBySqlAsync("update ABG_RateCache set searchcount=searchcount+1 where SearchMD5 = @SearchMD5", new { SearchMD5 = md5Key });
                        return JsonConvert.DeserializeObject<List<StdVehicle>>(cache);
                    }
                }
                List<StdVehicleExtend> firstList = new List<StdVehicleExtend>();
                var locList = await GetAllLocation();
                var (startLocList, endLocList) = await CommonLocationHelper.GetLocaiton(vehicleRQ, locList);
                if (startLocList.Count == 0)
                {
                    return res;
                }
                var codeList = startLocList.Select(n => $"'{n.SuppLocId}'").ToList();
                codeList.AddRange(endLocList.Select(n => $"'{n.SuppLocId}'").ToList());

                var youngDriverList = await _yourDriverRepository.GetListBySqlAsync($"select * from Abg_YoungDriver where code in ({string.Join(",", codeList.Distinct().ToList())})", null);

                //var youngDriverList = await GetAllYoungDriverAsync();
                //usertodo 需要改成并发
                var iataList = _setting.SupplierInfos;
                int batchSize = 10; // 限制 10个任务跑
                List<Task> runTasks = new List<Task>();
                foreach (var iata in iataList)
                {
                    var supplierSloc = startLocList.Where(n => n.Vendor == (int)EnumHelper.GetEnumTypeByStr<EnumCarVendor>(iata.Vendor)).ToList();

                    var supplierRLoc = endLocList.Where(n => n.Vendor == (int)EnumHelper.GetEnumTypeByStr<EnumCarVendor>(iata.Vendor)).ToList();
                    foreach (var start in supplierSloc)
                    {
                        if (vehicleRQ.PickUpLocationCode == vehicleRQ.ReturnLocationCode)
                        {
                            QueryDto queryDto = new QueryDto
                            {
                                SupplierInfo = iata,
                                PickUpDateTime = vehicleRQ.PickUpDateTime,
                                ReturnDateTime = vehicleRQ.ReturnDateTime,
                                PickUpLocationCode = start.VendorLocId,
                                ReturnLocationCode = start.VendorLocId,
                                DriverAge = vehicleRQ.DriverAge,
                                CitizenCountryCode = vehicleRQ.CitizenCountryCode,
                                startLoc = start,
                                endLoc = start,
                                youngDriverList = youngDriverList
                            };
                            var task = Task.Run(async () =>
                            {
                                var rq = await GetVehiclesByloc(queryDto);
                                firstList.AddRange(rq);
                            });
                            runTasks.Add(task);
                            if (runTasks.Count >= batchSize)
                            {
                                await Task.WhenAll(runTasks);
                                runTasks = new List<Task>();
                            }
                        }
                        else
                        {
                            foreach (var end in supplierRLoc)
                            {
                                QueryDto queryDto = new QueryDto
                                {
                                    SupplierInfo = iata,
                                    PickUpDateTime = vehicleRQ.PickUpDateTime,
                                    ReturnDateTime = vehicleRQ.ReturnDateTime,
                                    PickUpLocationCode = start.VendorLocId,
                                    ReturnLocationCode = end.VendorLocId,
                                    DriverAge = vehicleRQ.DriverAge,
                                    CitizenCountryCode = vehicleRQ.CitizenCountryCode,
                                    startLoc = start,
                                    endLoc = end,
                                    youngDriverList = youngDriverList
                                };
                                var task = Task.Run(async () =>
                                {
                                    var rq = await GetVehiclesByloc(queryDto);
                                    firstList.AddRange(rq);
                                });
                                runTasks.Add(task);
                                if (runTasks.Count > batchSize)
                                {
                                    await Task.WhenAll(runTasks);
                                    runTasks = new List<Task>();
                                }
                            }
                        }
                    }
                }
                await Task.WhenAll(runTasks);
                ParallelOptions options = new ParallelOptions();
                options.MaxDegreeOfParallelism = 10; // 设置最大线程数为 4
                //并发请求raterule规则构建实体
                Parallel.ForEach(firstList, options, async item =>
                {
                    await BuildTermsAndConditions(item);
                    // 在这里执行一些操作
                });
                if (_setting.PassMin > 0 && res.Count > 0)
                {
                    var dbCache = GZipHelper.Compress(JsonConvert.SerializeObject(res));
                    var rateMD5 = Md5Helper.ComputeMD5Hash(dbCache);
                    if (dbModel != null)
                    {
                        await _rateCacheRepository.UpdateBySqlAsync("update ABG_RateCache set searchcount=searchcount+1,RateMD5=@RateMD5,RateCache=@RateCache,PreUpdateTime=updatetime,updatetime=@updatetime,ExpireTime=@ExpireTime where SearchMD5=@SearchMD5",
                            new { RateMD5 = rateMD5, RateCache = dbCache, updatetime = DateTime.Now, ExpireTime = DateTime.Now.AddMinutes(_setting.PassMin), SearchMD5 = md5Key });
                    }
                    else
                    {
                        ABGRateCache aCERateCache = new ABGRateCache
                        {
                            SearchMD5 = md5Key,
                            SearchKey = cache_key,
                            SearchCount = 1,
                            RateCache = dbCache,
                            RateMD5 = rateMD5,
                            CanSaleCount = res.Count,
                            UpdateTime = DateTime.Now,
                            PreUpdateTime = DateTime.Now,
                            ExpireTime = DateTime.Now.AddMinutes(_setting.PassMin)
                        };
                        await _rateCacheRepository.InsertAsync(aCERateCache);
                    }
                }
                res = _mapper.Map<StdVehicleExtend, StdVehicle>(firstList);

                return res;
            }
            catch (Exception ex)
            {
                Log.Error($"APIERR:{ex.Message}");
                return res;
            }
            finally
            {
                stopwatch.Stop();
                Log.Information($"耗时{stopwatch.ElapsedMilliseconds}ms");
            }
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="queryOrderRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout)
        {
            try
            {
                var proOrder = await _proOrdRepository.GetByIdAsync("select * from ABG_CarProReservation where orderNo = @orderNo", new { orderNo = queryOrderRQ.OrderNo });
                if (proOrder == null)
                {
                    return new StdQueryOrderRS
                    {
                        IsSuccess = false,
                        ErrorMessage = "找不到对应的供应商订单"
                    };
                }
                var postData = new ABG_OTA_VehRetResRQ
                {
                    Version = "1.0",
                    POS = new POS
                    {
                        Source = new Source
                        {
                            RequestorID = new RequestorID { ID = proOrder.VendorCode, Type = 5 }
                        }
                    },
                    VehRetResRQCore = new VehRetResRQCore
                    {
                        UniqueID = new UniqueID
                        {
                            Type = 14,
                            ID = proOrder.ReservationId
                        },
                        PersonName = new PersonName
                        {
                            Surname = queryOrderRQ.LastName
                        }
                    },
                    VehRetResRQInfo = new VehRetResRQInfo
                    {
                        Vendor = new Vendor { CompanyShortName = proOrder.VendorName },
                    }
                };
                return await QueryOrderAsync(proOrder, postData);
            }
            catch (Exception ex)
            {
                Log.Error($"APIERR:{ex.Message}");
                return new StdQueryOrderRS();
            }
        }

        public async Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            try
            {
                var proOrder = await _proOrdRepository.GetByIdAsync("select * from ABG_CarProReservation where orderNo = @orderNo", new { orderNo = cancelOrderRQ.OrderNo });
                if (proOrder == null)
                {
                    return new StdCancelOrderRS
                    {
                        CancelSuc = false,
                        Message = "找不到对应的供应商订单"
                    };
                }
                ABG_OTAVehCancelRQ cancelRq = new ABG_OTAVehCancelRQ
                {
                    Version = "1.0",
                    POS = new POS
                    {
                        Source = new Source
                        {
                            RequestorID = new RequestorID { ID = proOrder.VendorCode, Type = 5 }
                        }
                    },
                    VehCancelRQCore = new VehCancelRQCore
                    {
                        CancelType = "Commit",
                        UniqueID = new UniqueID
                        {
                            Type = 15,
                            ID = proOrder.ReservationId
                        },
                        PersonName = new PersonName
                        {
                            Surname = cancelOrderRQ.LastName
                        }
                    },
                    VehCancelRQInfo = new VehCancelRQInfo
                    {
                        Vendor = new Vendor { CompanyShortName = proOrder.VendorName },
                    }
                };
                return await CancelOrderAsync(proOrder, cancelRq);
            }
            catch (Exception ex)
            {
                Log.Error($"APIERR:{ex.Message}");
                return new StdCancelOrderRS();
            }
        }

        /// <summary>
        /// 创建订单接口
        /// </summary>
        /// <param name="createOrderRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            try
            {
                //解析ratecode  0122511R_PER_PER_1_4_CCAR3_1E
                var ratecodeList = createOrderRQ.RateCode.Split("_");

                SecSupplier secSupplier = null;
                foreach (var su in _setting.SupplierInfos)
                {
                    secSupplier = su.secSuppliers.FirstOrDefault(n => n.VerdorType.ToString() == ratecodeList[0]);
                    if (secSupplier != null)
                    {
                        break;
                    }
                }

                if (secSupplier == null)
                {
                    throw new Exception("供应商解析不正确");
                }
                var dbOrder = await _proOrdRepository.GetByIdAsync(createOrderRQ.OrderNo);
                if (dbOrder != null)
                {
                    return new StdCreateOrderRS()
                    {
                        OrderSuc = false,
                        SuppOrderId = dbOrder.ReservationId,
                        Message = $"已存在订单{dbOrder.OrderNo}"
                    };
                }
                var postModel = new ABG_OTA_VehResRQ
                {
                    Version = "1.0",
                    POS = new POS
                    {
                        Source = new Source
                        {
                            RequestorID = new RequestorID { ID = secSupplier.IATA, Type = 5 }
                        }
                    },
                    VehResRQCore = new VehResRQCore
                    {
                        VehRentalCore = new VehRentalCore
                        {
                            PickUpDateTime = createOrderRQ.PickUpDateTime,
                            ReturnDateTime = createOrderRQ.ReturnDateTime,
                            PickUpLocation = new Location { LocationCode = ratecodeList[1] },
                            ReturnLocation = new Location { LocationCode = ratecodeList[2] },
                        },
                        Customer = new Customer
                        {
                            Primary = new Primary
                            {
                                PersonName = new PersonName
                                {
                                    GivenName = createOrderRQ.FirstName,
                                    //MiddleName=createOrderRQ.LastName, 中间名没有
                                    Surname = createOrderRQ.LastName,
                                },
                                CitizenCountryName = new CitizenCountryName { Code = createOrderRQ.CitizenCountryCode }
                            }
                        },
                        VendorPref = new VendorPref { CompanyShortName = secSupplier.Vendor },
                        VehPref = new CreateVehPref
                        {
                            AirConditionPref = "Preferred", //指空调偏好
                            ClassPref = "Preferred",  //指车辆等级偏好
                            TransmissionPref = "Preferred", //指变速箱偏好。
                            TransmissionType = "Automatic", //指变速箱类型。
                            TypePref = "Preferred", //指车辆类型偏好
                            VehType = new CreateVehType { VehicleCategory = Convert.ToInt32(ratecodeList[3]) },
                            VehClass = new VehClass { Size = Convert.ToInt32(ratecodeList[4]) },
                            VehGroup = new VehGroup { GroupValue = ratecodeList[5], GroupType = "SIPP" },
                        },

                        RateQualifier = new RateQualifier { RateQualifierValue = ratecodeList.LastOrDefault() }
                    }
                };
                if (createOrderRQ.Extras != null && createOrderRQ.Extras.Count > 0)
                {
                    List<SpecialEquipPref> equips = new List<SpecialEquipPref>();
                    foreach (var item in createOrderRQ.Extras)
                    {
                        SpecialEquipPref eq = new SpecialEquipPref
                        {
                            EquipType = GetSupplierEquipType(item.EquipType),
                            Quantity = item.Quantity.ToString()
                        };
                        if (eq.EquipType == "")
                        {
                            continue;
                        }
                        equips.Add(eq);
                    }
                    if (equips.Count > 0)
                    {
                        postModel.VehResRQCore.SpecialEquipPrefs = new SpecialEquipPrefs { SpecialEquipPrefList = equips };
                    }
                }
                if (createOrderRQ.DriverAge > 0)
                {
                    postModel.VehResRQCore.DriverType = new DriverType { Age = createOrderRQ.DriverAge };
                }
                return await CreateOrderAsync(createOrderRQ, secSupplier, postModel);
            }
            catch (Exception ex)
            {
                Log.Error($"APIERR:{ex.Message}");
                return new StdCreateOrderRS();
            }
        }

        #endregion 业务接口

        #region FTP落地数据 location, Location Opening Times

        public async Task<bool> InitLocation()
        {
            //await RunLoation();
            await PushLocationToDb();
            return true;
        }

        public async Task<bool> InitLocationOperationTimes()
        {
            var locList = await _locRepository.GetAllAsync();
            var list = locList.ToList();
            foreach (var item in _setting.SupplierInfos)
            {
                var dbList = await _locRepository.GetAllAsync();
                var filename = "Locs_hrs.dat";
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{datPath}/{item.Vendor}/{filename}");
                await BuildLocationOperationTimes(path, list);
            }
            return true;
        }

        /// <summary>
        /// todooooo
        /// </summary>
        /// <returns></returns>
        public async Task InitSupplierLocaiton()
        {
            var spLoc = await _supplierCatRe.GetListBySqlAsync("select * from CarRental.dbo.Car_Location_Suppliers where supplier =@supplier", new { supplier = (int)EnumCarSupplier.ABG });
            var allCity = await _CatCityRe.GetAllAsync();
            var locList = await _locRepository.GetAllAsync();
            foreach (var item in locList)
            {
                /* if (item.Latitude == null || item.Longitude == null)
                 {
                     continue;
                 }*/
                var vendor = (int)EnumHelper.GetEnumTypeByStr<EnumCarVendor>(item.VendorName);
                var spModel = spLoc.FirstOrDefault(n => n.SuppLocId == item.LocationCode && n.Vendor == vendor);
                var cityid = 0;
                if (!String.IsNullOrWhiteSpace(item.City))
                {
                    var city = allCity.FirstOrDefault(n => n.CityNameEn.ToLower() == item.City.ToLower());
                    if (city != null)
                    {
                        cityid = city.CityId;
                    }
                }
                if (spModel == null)
                {
                    CarLocationSupplier newitem = new CarLocationSupplier
                    {
                        LocationName = item.LocationName,
                        CountryCode = "US",
                        CountryName = "",
                        CityId = cityid,
                        CityName = item.City,
                        Address = item.Address,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        Airport = string.IsNullOrWhiteSpace(item.APOCode) ? false : true,
                        AirportCode = item.APOCode,
                        RailwayStation = item.RentalType == "Railway/Bus Station" ? true : false,
                        PostalCode = item.Postcode,
                        Telephone = item.PhoneNumber,
                        Email = item.Email,
                        OperationTime = item.OperationTimes,
                        Supplier = ((int)EnumCarSupplier.ABG).ToString(),
                        SuppLocId = item.LocationCode,
                        Vendor = (int)EnumHelper.GetEnumTypeByStr<EnumCarVendor>(item.VendorName),
                        VendorLocId = item.LocationCode,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    await _supplierCatRe.InsertAsync(newitem);
                }
                else
                {
                    spModel.LocationName = item.LocationName;
                    spModel.CityId = cityid;
                    spModel.CityName = item.City;
                    spModel.Address = item.Address;
                    spModel.Latitude = item.Latitude;
                    spModel.Longitude = item.Longitude;
                    spModel.Airport = string.IsNullOrWhiteSpace(item.APOCode) ? false : true;
                    spModel.AirportCode = item.APOCode;
                    spModel.RailwayStation = item.RentalType == "Railway/Bus Station" ? true : false;
                    spModel.PostalCode = item.Postcode;
                    spModel.Telephone = item.PhoneNumber;
                    spModel.Email = item.Email;
                    spModel.OperationTime = item.OperationTimes;
                    spModel.Supplier = ((int)EnumCarSupplier.ABG).ToString();
                    spModel.SuppLocId = item.LocationCode;
                    spModel.Vendor = (int)EnumHelper.GetEnumTypeByStr<EnumCarVendor>(item.VendorName);
                    spModel.VendorLocId = item.LocationCode;
                    //spModel.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    spModel.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    await _supplierCatRe.UpdateAsync(spModel, "status");
                }
            }
        }

        public async Task PushLocationToDb()
        {
            //待改成 批量插入
            foreach (var item in _setting.SupplierInfos)
            {
                var dbList = await _locRepository.GetAllAsync();
                var filename = "Locs.dat";
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{datPath}/{item.Vendor}/{filename}");
                await BuildLocationByFile(path, item.Vendor, dbList);
            }
        }

        /// <summary>
        /// 构建门店信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public async Task BuildLocationByFile(string path, string companyName, List<ABGLocation> locList)
        {
            //如果文件存在
            if (File.Exists(path))
            {
                //读取文件内容
                IEnumerable<string> line = File.ReadLines(path);
                foreach (var item in line)
                {
                    if (item.Contains("LOCS"))
                    {
                        continue;
                    }
                    try
                    {
                        var spiltLine = item.Split("\t").ToList();
                        if (spiltLine.Count <= 27)
                        {
                            List<string> newList = new List<string>();
                            for (var i = 0; i < spiltLine.Count; i++)
                            {
                                if (i == 21)
                                {
                                    newList.Add("");
                                }
                                newList.Add(spiltLine[i]);
                            }
                            spiltLine = newList;
                        }
                        ABGLocation location = new ABGLocation();
                        SetPropertiesFromArray(location, spiltLine, 1);

                        location.VendorName = companyName;
                        var dbModel = locList.FirstOrDefault(n => n.LocationCode == location.LocationCode);
                        var hashKey = GZipHelper.GetSHA256Hash(item);
                        location.hashKey = hashKey;
                        if (dbModel == null)
                        {
                            location.CreateTime = DateTime.Now;

                            await _locRepository.InsertAsync(location);
                        }
                        else
                        {
                            if (dbModel.hashKey != hashKey)
                            {
                                location.LocationCode = dbModel.LocationCode;
                                location.UpdateTime = DateTime.Now;
                                await _locRepository.UpdateAsync(location, "CreateTime,OperationTimes");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Information($"初始化地址失败{item}");
                    }
                }
            }
        }

        public List<StdTime> BuildStdTime(string[] spiltLine, int index)
        {
            List<StdTime> res = new List<StdTime>();

            if (spiltLine[index] != spiltLine[index + 1])
            {
                StdTime stdTime = new StdTime();
                stdTime.Start = spiltLine[index];
                stdTime.End = spiltLine[index + 1];
                res.Add(stdTime);
            }
            if (spiltLine[index + 2] != spiltLine[index + 3])
            {
                StdTime stdTime = new StdTime();
                stdTime.Start = spiltLine[index + 2];
                stdTime.End = spiltLine[index + 3];
                res.Add(stdTime);
            }
            return res;
        }

        /// <summary>
        /// 根据静态文件构建营业时间
        /// </summary>
        /// <param name="path"></param>
        /// <param name="locList"></param>
        /// <returns></returns>
        public async Task BuildLocationOperationTimes(string path, List<ABGLocation> locList)
        {
            //如果文件存在
            if (File.Exists(path))
            {
                //读取文件内容
                IEnumerable<string> line = File.ReadLines(path);
                foreach (var item in line)
                {
                    if (item.Contains("HOURS"))
                    {
                        continue;
                    }
                    try
                    {
                        var spiltLine = item.Split("\t");
                        List<StdOperationTime> all = new List<StdOperationTime>();

                        all.Add(new StdOperationTime
                        {
                            Week = EnumCarWeek.Monday,
                            Times = BuildStdTime(spiltLine, 7)
                        });
                        all.Add(new StdOperationTime
                        {
                            Week = EnumCarWeek.Tuesday,
                            Times = BuildStdTime(spiltLine, 11)
                        });
                        all.Add(new StdOperationTime
                        {
                            Week = EnumCarWeek.Wednesday,
                            Times = BuildStdTime(spiltLine, 15)
                        });
                        all.Add(new StdOperationTime
                        {
                            Week = EnumCarWeek.Thursday,
                            Times = BuildStdTime(spiltLine, 19)
                        });
                        all.Add(new StdOperationTime
                        {
                            Week = EnumCarWeek.Friday,
                            Times = BuildStdTime(spiltLine, 23)
                        });
                        all.Add(new StdOperationTime
                        {
                            Week = EnumCarWeek.Saturday,
                            Times = BuildStdTime(spiltLine, 27)
                        });
                        all.Add(new StdOperationTime
                        {
                            Week = EnumCarWeek.Sunday,
                            Times = BuildStdTime(spiltLine, 3)
                        });
                        var json = JsonConvert.SerializeObject(all);
                        var hashKey = GZipHelper.GetSHA256Hash(json);
                        var dbmodel = locList.FirstOrDefault(n => n.LocationCode == spiltLine[1]);
                        if (dbmodel != null && dbmodel.operationtimehashkey != hashKey)
                        {
                            await _locRepository.UpdateBySqlAsync($"update abg_location_new set OperationTimes=@json,operationtimehashkey=@hashkey  where locationcode = @locationcode", new { json = json, locationcode = spiltLine[1], hashkey = hashKey });
                            Log.Information($"update 【{spiltLine[1]}】OperationTimes success!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"更新开张时间报错");
                    }
                }
            }
        }

        /// <summary>
        /// 信用卡个数要求政策
        /// </summary>
        /// <returns></returns>
        public async Task InitCreditCardPolicy()
        {
            //待改成 批量插入
            foreach (var item in _setting.SupplierInfos)
            {
                var dbList = await _cardRepository.GetAllAsync();
                var filename = "CreditCard.dat";
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{datPath}/{item.Vendor}/{filename}");
                await BuildCreditCardPolicyByFile(path, dbList);
            }
        }

        /// <summary>
        /// ftp 初始化信用卡政策
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task BuildCreditCardPolicyByFile(string path, List<ABG_CreditCardPolicy> dbList)
        {
            //如果文件存在
            if (File.Exists(path))
            {
                //读取文件内容
                IEnumerable<string> line = File.ReadLines(path);
                foreach (var item in line)
                {
                    if (item.Contains("Credit"))
                    {
                        continue;
                    }
                    try
                    {
                        var spiltLine = item.Split("\t").ToList();
                        ABG_CreditCardPolicy aBG_CreditCard = new ABG_CreditCardPolicy();
                        SetPropertiesFromArray(aBG_CreditCard, spiltLine, 0);
                        var dbmodel = dbList.FirstOrDefault(n => n.AvisLocationCode == aBG_CreditCard.AvisLocationCode && n.AvisCarGroup == aBG_CreditCard.AvisCarGroup);
                        var hashKey = GZipHelper.GetSHA256Hash(item);
                        aBG_CreditCard.hashkey = hashKey;
                        if (dbmodel == null)
                        {
                            aBG_CreditCard.CreatTime = DateTime.Now;
                            if (string.IsNullOrWhiteSpace(aBG_CreditCard.AvisLocationCode))
                            {
                                continue;
                            }
                            await _cardRepository.InsertAsync(aBG_CreditCard);
                        }
                        else
                        {
                            if (dbmodel.hashkey != hashKey)
                            {
                                aBG_CreditCard.ID = dbmodel.ID;

                                await _cardRepository.UpdateAsync(aBG_CreditCard, "CreatTime");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"信用卡政策插入失败{ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 初始化年龄政策
        /// </summary>
        /// <returns></returns>
        public async Task InitYoungDriver()
        {
            //FTPDownLoad();
            //找出所有的db
            var dbList = await GetAllYoungDriverAsync();
            foreach (var item in _setting.SupplierInfos)
            {
                var filename = "YoungDriver.dat";
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{datPath}/{item.Vendor}/{filename}");
                await InitYoungDriverByFile(path, dbList);
            }
        }

        public const string datPath = "Supplier/ABG/Data";

        public async Task InitYoungDriverByFile(string path, List<AbgYoungDriver> list)
        {
            //如果文件存在
            if (File.Exists(path))
            {
                //读取文件内容
                IEnumerable<string> line = File.ReadLines(path);
                foreach (var item in line)
                {
                    if (item.Contains("Young"))
                    {
                        continue;
                    }
                    try
                    {
                        Log.Information($"item :{item}");
                        var spiltLine = item.Split("\t").ToList();
                        var carGroupIndex = 6;

                        if (spiltLine.Count == 37)
                        {
                            spiltLine.RemoveAt(6);
                            spiltLine.RemoveAt(6);
                            spiltLine.RemoveAt(6);
                        }
                        AbgYoungDriver youngDriver = new AbgYoungDriver();
                        SetPropertiesFromArray(youngDriver, spiltLine, 1);
                        var enHash = GZipHelper.GetSHA256Hash(item);
                        var dbModel = list.FirstOrDefault(n => n.Code == youngDriver.Code && n.CarGroup == youngDriver.CarGroup);
                        if (dbModel == null)
                        {
                            youngDriver.HashKey = enHash;
                            await _yourDriverRepository.InsertAsync(youngDriver);
                        }
                        else
                        {
                            youngDriver.ID = dbModel.ID;
                            await _yourDriverRepository.UpdateAsync(dbModel);
                            //update todo
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"插入youngDriver失败{ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 根据反射对属性进行赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="values"></param>
        /// <param name="valueIndex"></param>
        private void SetPropertiesFromArray<T>(T obj, List<string> values, int valueIndex = 0)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            // int valueIndex = 0; // 初始化索引从0开始

            foreach (var property in properties)
            {
                // 如果属性有 [DatabaseGenerated] 特性且类型为 Identity，则跳过
                if (property.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                {
                    continue;
                }

                if (valueIndex >= values.Count) break; // 防止数组越界

                var value = values[valueIndex];
                if (string.IsNullOrWhiteSpace(value))
                {
                    valueIndex++;
                    continue;
                }

                // 检查属性类型并设置值
                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(obj, Convert.ToInt32(value));
                }
                else if (property.PropertyType == typeof(int?))
                {
                    property.SetValue(obj, string.IsNullOrWhiteSpace(value) ? (int?)null : Convert.ToInt32(value));
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    property.SetValue(obj, Convert.ToDecimal(value));
                }
                else if (property.PropertyType == typeof(decimal?))
                {
                    property.SetValue(obj, string.IsNullOrWhiteSpace(value) ? (decimal?)null : Convert.ToDecimal(value));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    property.SetValue(obj, DateTime.Parse(value));
                }
                else if (property.PropertyType == typeof(DateTime?))
                {
                    property.SetValue(obj, DateTime.TryParse(value, out var dateTime) ? (DateTime?)dateTime : null);
                }
                else
                {
                    property.SetValue(obj, value);
                }

                valueIndex++;
            }
        }

        public async Task FTPInit()
        {
            foreach (var item in _setting.SupplierInfos)
            {
                var descpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{datPath}/{item.Vendor}");
                if (!Directory.Exists(descpath))
                {
                    // Ensure local folder path exists
                    Directory.CreateDirectory(descpath);
                }
                var filesToDownload = item.FTP.FTPFiles;
                foreach (var f in filesToDownload)
                {
                    var ofile = $"{item.FTP.FTPUrl}/{f}";
                    var descfilename = $"{descpath}/{f}";
                    FTPHelper.DownloadFile(ofile, descfilename, item.FTP.Name, item.FTP.PassWord);
                }
            }
            // 落地门店
            await InitLocation();
            await InitLocationOperationTimes();
            await InitSupplierLocaiton();
            await InitCreditCardPolicy();
            await InitYoungDriver();
            ABGCacheInstance.Instance.SetYoungDriver(new List<AbgYoungDriver>());
            ABGCacheInstance.Instance.SetLocation(new List<CarLocationSupplier>());
        }

        #endregion FTP落地数据 location, Location Opening Times

        #region

        public async Task RunLoation()
        {
            var location = await GetAllLocation();
            var iataList = _setting.SupplierInfos;
            List<StdVehicle> res = new List<StdVehicle>();
            int batchSize = 4; // 限制 10个任务跑
            List<Task> runTasks = new List<Task>();
            var youngDriverList = await _yourDriverRepository.GetAllAsync();
            foreach (var loc in location)
            {
                foreach (var iata in iataList)
                {
                    QueryDto queryDto = new QueryDto
                    {
                        SupplierInfo = iata,
                        PickUpDateTime = DateTime.Now.AddDays(60),
                        ReturnDateTime = DateTime.Now.AddDays(61),
                        PickUpLocationCode = loc.VendorLocId,
                        ReturnLocationCode = loc.VendorLocId,
                        // DriverAge = vehicleRQ.DriverAge,
                        CitizenCountryCode = "US",
                        startLoc = loc,
                        endLoc = loc,
                        youngDriverList = youngDriverList
                    };
                    var task = Task.Run(async () =>
                    {
                        var rq = await GetVehiclesByloc(queryDto);
                        res.AddRange(rq);
                    });
                    runTasks.Add(task);
                    if (runTasks.Count > batchSize)
                    {
                        await Task.WhenAll(runTasks);
                        runTasks = new List<Task>();
                    }
                    if (res.Count > 0)
                    {
                        var ssss = 2323;
                    }
                }
            }
        }

        #endregion
    }
}

public class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}