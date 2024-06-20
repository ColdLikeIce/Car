using CommonCore.Mapper;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using HeyTripCarWeb.Supplier.ABG.Util;
using HeyTripCarWeb.Supplier.ACE.Models.Dbs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.ABG
{
    public class ABGApi : ICarSupplierApi
    {
        private readonly IRepository<AceLocation> _locRepository;
        private readonly ABGAppSetting _setting;
        private readonly IMapper _mapper;

        public ABGApi(IOptions<ABGAppSetting> options, IMapper mapper, IRepository<AceLocation> locRepository)
        {
            _setting = options.Value;
            _mapper = mapper;
            _locRepository = locRepository;
        }

        #region 原始接口

        /// <summary>
        /// 查询车辆集合。
        /// 支持：IATA/AirportID/CityID/Geo
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> VehAvailRate(ABG_OTA_VehAvailRateRQ availRateRQ, int timeout = 45000)
        {
            var res = BuildEnvelope(new CommonRequest { OTA_VehAvailRateRQ = availRateRQ, Type = 1 });
            var model = ABGXmlHelper.GetResponse<ABG_OTA_VehAvailRateRS>(res);
            if (model.Errors != null && model.Errors?.ErrorList.Count > 0)
            {
                throw new Exception($"出现错误");
            }
            if (model == null)
            {
            }
            var rentCore = model.VehAvailRSCore.VehRentalCore;
            var avails = model.VehAvailRSCore.VehVendorAvails.VehVendorAvail;
            foreach (var avail in avails)
            {
                var veh = avail.VehAvails.VehAvail;
                foreach (var ve in veh)
                {
                    var veCore = ve.VehAvailCore;
                    if (veCore.Status == "Available")
                    {
                        StdVehicle std = new StdVehicle();
                        std.Supplier = EnumCarSupplier.ABG;

                        std.VehicleCode = veCore.Vehicle?.VehMakeModel?.Code;
                        std.ProductCode = $"{EnumCarSupplier.ABG}_usertodo_{std.VehicleCode}";
                        std.VehicleName = veCore.Vehicle?.VehMakeModel?.Name;
                        std.DoorCount = (EnumCarDoorCount)veCore.Vehicle.VehType.DoorCount;
                        //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、
                        //8（旅行车）或 9（皮卡）
                        std.VehicleCategory = BuildCarType(veCore.Vehicle.VehType.VehicleCategory.ToString());
                        // 尺寸必须等于以下产品之一：1（小型）、2（小型型）、3（经济型）、4（紧凑型）、
                        //  5（中型）、6（中级）、7（标准）、8（全尺寸）、9（豪华）、10（高级）或11（小型货车）。
                        std.VehicleClass = (EnumCarVehicleClass)veCore.Vehicle.VehClass.Size;
                        std.AirConditioning = veCore.Vehicle.AirConditionInd;
                        if (!string.IsNullOrWhiteSpace(veCore.Vehicle.DriveType))
                        {
                            Log.Information($"驱动类型{veCore.Vehicle.DriveType}");
                            std.DriveType = EnumCarDriveType.None; //驱动类型是啥  usertodo
                        }
                        if (!string.IsNullOrWhiteSpace(veCore.Vehicle.FuelType))
                        {
                            Log.Information($"燃料类型{veCore.Vehicle.FuelType}");
                            //std.FuelType = veCore.Vehicle.FuelType//usertodo
                        }
                        if (!string.IsNullOrWhiteSpace(veCore.Vehicle.PassengerQuantity))
                        {
                            Log.Information($"乘客数量{veCore.Vehicle.PassengerQuantity}");
                            std.PassengerQuantity = Convert.ToInt32(veCore.Vehicle.PassengerQuantity);
                        }
                        if (veCore.Vehicle.BaggageQuantity > 0)
                        {
                            Log.Information($"行李数量{veCore.Vehicle.BaggageQuantity}");
                            std.PassengerQuantity = veCore.Vehicle.BaggageQuantity;
                        }
                        std.PictureURL = veCore.Vehicle.PictureURL;
                        std.TransmissionType = veCore.Vehicle.TransmissionType == "Manual" ? EnumCarTransmissionType.Manual : EnumCarTransmissionType.Automatic;

                        std.MinDriverAge = 0; //userloss
                        std.MaxDriverAge = 99; //userloss
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
                            var charges = rentalRate.VehicleCharges.VehicleCharge.Where(n => n.Purpose == "6").ToList();
                            if (charges.Count > 0)
                            {
                                Log.Information($"charges数量为{charges}");
                                rateinfo.Currency = charges.FirstOrDefault().CurrencyCode;
                                rateinfo.Amount = charges.FirstOrDefault().Amount;
                                rateinfo.Description = charges.FirstOrDefault().Description;
                            }
                        }
                        std.RateDistance = rateinfo;
                        //价格

                        std.TotalCharge = null;

                        var fuelPolicy = rentalRate.VehicleCharges.VehicleCharge.Where(n => n.Description.Contains("FUEL POLICY")).ToList();
                        if (fuelPolicy.Count > 0)
                        {
                            //燃油政策
                            if (fuelPolicy.Exists(n => n.Description.Contains("Full To Full")))
                            {
                                std.FuelPolicy = EnumCarFuelPolicy.FullToFull;
                            }
                            else
                            {
                                Log.Information($"燃油政策:{JsonConvert.SerializeObject(fuelPolicy)}");
                            }
                        }
                        //
                        var rateRule = await GetRateRule(availRateRQ, veCore);
                        if (rateRule != null)
                        {
                            //保险
                            List<StdPricedCoverage> stdPricedCoverages = new List<StdPricedCoverage>();
                            foreach (var pricedCoverage in rateRule.PricedCoverages)
                            {
                                var charge = pricedCoverage.Charge;
                                StdPricedCoverage stdPriced = new StdPricedCoverage()
                                {
                                    CoverageType = BuildEnumCarCoverageType(pricedCoverage.Coverage.Code),
                                    CoverageDescription = pricedCoverage.Coverage.CoverageDetails.Description,
                                    Description = pricedCoverage.Coverage.CoverageDetails.Description,
                                    CurrencyCode = charge.CurrencyCode,
                                    Amount = charge.Amount,
                                    TaxInclusive = charge.TaxInclusive,
                                    IncludedInRate = charge.IncludedInRate,
                                    //IncludedInEstTotalInd = false, //usertodo
                                    Calculation = new StdCalculation
                                    {
                                        Quantity = charge.Calculation.Quantity,
                                        UnitName = charge.Calculation.UnitName,
                                        UnitCharge = charge.Calculation.UnitCharge
                                    },
                                    MaxCharge = charge.MinMax.MaxCharge,
                                    MinCharge = charge.MinMax.MinCharge,
                                };
                                stdPricedCoverages.Add(stdPriced);
                            }
                            std.PricedCoverages = stdPricedCoverages;
                            List<StdVehicleCharge> vehCharges = new List<StdVehicleCharge>();
                            var vehChargesList = rateRule.RentalRate.VehicleCharges.VehicleCharge;
                            var FeesList = vehChargesList.Where(n => n.IncludedInRate).ToList();
                            List<StdFee> stdfee = new List<StdFee>();
                            foreach (var fe in FeesList)
                            {
                                var stdFee = _mapper.Map<VehicleCharge, StdFee>(fe);
                                stdfee.Add(stdFee);
                            }
                            std.Fees = stdfee;
                            //车辆收费
                            var vehicleChargesList = vehChargesList.Where(n => !n.IncludedInRate).ToList();
                            foreach (var item in vehicleChargesList)
                            {
                                Log.Information($"车辆收费信息{JsonConvert.SerializeObject(item)}");
                                StdVehicleCharge newItem = new StdVehicleCharge
                                {
                                    Purpose = EnumCarCoverageType.OtherTaxesAndServiceCharges,
                                    PurposeDescription = item.Description,
                                    Description = item.Description,
                                    CurrencyCode = item.CurrencyCode,
                                    Amount = item.Amount,
                                    TaxInclusive = item.TaxInclusive,
                                };
                                vehCharges.Add(newItem);
                            }
                            std.VehicleCharges = vehCharges;
                        }
                    }
                }
            }
            return null;
        }

        public EnumCarCoverageType BuildEnumCarCoverageType(string code)
        {
            switch (code)
            {
                case "ALI":
                    return EnumCarCoverageType.LiabilityInsuranceSupplement;

                case "PEP":
                    return EnumCarCoverageType.PersonalEffectsCoverage;

                case "CDW":
                    return EnumCarCoverageType.CollisionDamageWaiver;

                case "PAI":
                    return EnumCarCoverageType.PersonalAccidentInsurance;

                default:
                    Log.Error($"usertodo{code}");
                    return EnumCarCoverageType.None;
            }
        }

        /// <summary>
        /// 获取费率规则
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="veCore"></param>
        /// <returns></returns>
        public async Task<ABG_OTA_VehRateRuleRS> GetRateRule(ABG_OTA_VehAvailRateRQ availRateRQ, VehAvailCore veCore)
        {
            var ruleRq = new OTA_VehRateRuleRQ
            {
                //返回请求数
                Version = "1.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID { ID = _setting.UserID, Type = 1 }
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
            var res = BuildEnvelope(new CommonRequest { OTA_VehRateRuleRQ = ruleRq, Type = 2 });
            var model = ABGXmlHelper.GetResponse<ABG_OTA_VehRateRuleRS>(res);
            return model;
        }

        public string BuildEnvelope(CommonRequest model)
        {
            ns_Request ns_Request = new ns_Request();
            switch (model.Type)
            {
                case 1:
                    ns_Request.OTA_VehAvailRateRQ = model.OTA_VehAvailRateRQ;
                    break;

                case 2:
                    ns_Request.OTA_VehRateRuleRQ = model.OTA_VehRateRuleRQ;
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
            return ABGXmlHelper.PostRequest(_setting.Url, envelope);
        }

        /// <summary>
        /// 构建车辆类型
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public EnumCarVehicleCategory BuildCarType(string category)
        {
            switch (category)
            {
                case "1":
                    return EnumCarVehicleCategory.Car;

                case "2":
                    return EnumCarVehicleCategory.Van;

                case "3":
                    return EnumCarVehicleCategory.SUV;

                case "4":
                    return EnumCarVehicleCategory.Convertible;

                case "8":
                    return EnumCarVehicleCategory.StationWagon;

                case "9":
                    return EnumCarVehicleCategory.Pickup;
            }
            Log.Error($"遇到其他车型{category}");
            return EnumCarVehicleCategory.Car;
        }

        #endregion 原始接口

        #region 业务接口

        /// <summary>
        /// 查询车辆集合。该供应商支持：目前只发现 IATA
        /// </summary>
        /// <param name="vehicleRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            //构建请求body 其他都一样
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
                        RequestorID = new RequestorID { ID = _setting.UserID, Type = 1 }
                    }
                },
                VehAvailRQCore = new VehAvailRQCore
                {
                    Status = "Available",
                    VehRentalCore = new VehRentalCore
                    {
                        //取车日期 还车日期
                        PickUpDateTime = Convert.ToDateTime(vehicleRQ.PickUpDateTime),
                        ReturnDateTime = Convert.ToDateTime(vehicleRQ.ReturnDateTime),
                        //出发地 目的地方
                        PickUpLocation = new Location { LocationCode = vehicleRQ.PickUpLocationCode },
                        ReturnLocation = new Location { LocationCode = vehicleRQ.ReturnLocationCode }
                    },
                    VendorPrefs = new VendorPrefs
                    {
                        VendorPref = new VendorPref { CompanyShortName = "Avis" }
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
                            VehClass = new VehClass { Size = 4 } //当 VehicleCategory 为 “Car(1)” 时，DoorCount 必须是 “2” 或 “4”。 其他值可忽略
                        }
                    },
                    //年龄
                    DriverType = new DriverType { Age = vehicleRQ.DriverAge },
                    //<!-- 包括全部价格：商务和休闲 -->
                    RateQualifier = new RateQualifier { RateCategory = "6" }
                },
                VehAvailRQInfo = new VehAvailRQInfo
                {
                    Customer = new Customer
                    {
                        //出生国家
                        Primary = new Primary
                        {
                            CitizenCountryName = new CitizenCountryName { Code = vehicleRQ.CitizenCountryCode }
                        }
                    }
                }
            };
            return await VehAvailRate(availRateRQ);
        }

        public Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout)
        {
            throw new NotImplementedException();
        }

        public Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        #endregion 业务接口
    }
}

public class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}