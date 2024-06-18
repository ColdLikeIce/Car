using CommonCore.Mapper;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.ACE.Config;
using HeyTripCarWeb.Supplier.ACE.Models.RQs;
using HeyTripCarWeb.Supplier.ACE.Models.RSs;
using HeyTripCarWeb.Supplier.ACE.Util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Drawing;
using System.Net;
using System.Text;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;
using static Dapper.SqlMapper;

namespace HeyTripCarWeb.Supplier.ACE
{
    public class ACEApi : IACEApi
    {
        private readonly AceAppSetting _setting;
        private readonly IMapper _mapper;

        public ACEApi(IOptions<AceAppSetting> options, IMapper mapper)
        {
            _setting = options.Value;
            _mapper = mapper;
        }

        #region 原始接口

        /// <summary>
        /// 查询车辆集合。
        /// 支持：IATA/AirportID/CityID/Geo
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> VehAvailRate(ACE_OTA_VehAvailRateRQ availRateRQ, int timeout = 45000)
        {
            List<StdVehicle> result = new List<StdVehicle>();
            var res = await BuildEnvelope(new CommonRequest { OTA_VehAvailRateRQ = availRateRQ, Type = 1 });
            var model = ACEXmlHelper.GetResponse<ACE_OTA_VehAvailRateRS>(res);
            if (model != null)
            {
                var rentCore = model.VehAvailRSCore.VehRentalCore;
                var avails = model.VehAvailRSCore.VehVendorAvails;
                foreach (var avail in avails)
                {
                    var veh = avail.VehAvails;
                    foreach (var ve in veh)
                    {
                        var veCore = ve.VehAvailCore;
                        if (veCore.Status == "Available")
                        {
                            StdVehicle std = new StdVehicle();
                            std.Supplier = EnumCarSupplier.None;
                            std.Vendor = EnumCarVendor.ACE_Rent_A_Car;
                            std.VehicleCode = veCore.Vehicle?.Code;
                            std.ProductCode = $"ACE_usertodo_{std.VehicleCode}";
                            std.VehicleName = veCore.Vehicle?.VehMakeModel?.Name;
                            std.DoorCount = BuildDoorCount(veCore.Vehicle.VehType.DoorCount);
                            //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、
                            //8（旅行车）或 9（皮卡）
                            std.VehicleCategory = (EnumCarVehicleCategory)veCore.Vehicle.VehType.VehicleCategory;// BuildCarType(veCore.Vehicle.VehType.VehicleCategory);
                            // 尺寸必须等于以下产品之一：1（小型）、2（小型型）、3（经济型）、4（紧凑型）、
                            //  5（中型）、6（中级）、7（标准）、8（全尺寸）、9（豪华）、10（高级）或11（小型货车）。
                            std.VehicleClass = (EnumCarVehicleClass)veCore.Vehicle.VehClass.Size;
                            std.AirConditioning = veCore.Vehicle.AirConditionInd;
                            if (!string.IsNullOrWhiteSpace(veCore.Vehicle.DriveType))
                            {
                                Log.Information($"驱动类型{veCore.Vehicle.DriveType}");
                                std.DriveType = EnumCarDriveType.None; //驱动类型是啥  usertodo
                            }
                            std.FuelType = (EnumCarFuelType)Enum.Parse(typeof(EnumCarFuelType), veCore.Vehicle.FuelType);
                            std.PassengerQuantity = veCore.Vehicle.PassengerQuantity;
                            std.PassengerQuantity = veCore.Vehicle.BaggageQuantity;
                            std.PictureURL = veCore.Vehicle.PictureURL;
                            std.TransmissionType = veCore.Vehicle.TransmissionType == "Manual" ? EnumCarTransmissionType.Manual : EnumCarTransmissionType.Automatic;

                            /*   std.MinDriverAge = 0; //userloss
                               std.MaxDriverAge = 99; //userloss*/
                            var rentalRate = veCore.RentalRate;
                            var rateDistance = rentalRate.RateDistance;
                            //里程限制
                            StdRateDistance rateinfo = new StdRateDistance
                            {
                                Unlimited = rateDistance.Unlimited,
                                DistUnitName = rateDistance.DistUnitName == "Km" ? EnumCarDistUnitName.Km : EnumCarDistUnitName.Mile,
                                Quantity = rateDistance.Quantity,
                                VehiclePeriodUnitName = (EnumCarPeriodUnitName)Enum.Parse(typeof(EnumCarPeriodUnitName), rateDistance.VehiclePeriodUnitName)
                            };
                            //Purpose枚举值 目的设置为：1租车2单程费用5升级5机场/城市/其他附加费5机场特许费5年龄附加费
                            //6额外距离收费6车辆登记费6以色列印花税7地方税7奥地利合同税7货物和服务税
                            //8额外里程/额外距离收费9额外10额外一周10天11小时13年龄附加费22预支付费用
                            //28可选费用
                            if (rentalRate.RateDistance.Unlimited == false)
                            {
                                var charges = rentalRate.VehicleCharges.Where(n => n.Purpose == "6").ToList();
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
                            var totalEl = veCore.TotalCharge;
                            var totalCharge = new StdTotalCharge
                            {
                                PayType = EnumCarPayType.Prepaid, //到付 usertodo RuleType
                                PriceType = EnumCarPriceType.NetRate, //底价模式
                                Currency = totalEl.CurrencyCode,
                                TotalAmount = totalEl.EstimatedTotalAmount,
                                RentalAmount = totalEl.RateTotalAmount
                            };
                            //其他费用
                            List<StdCurrencyAmount> stdAmount = new List<StdCurrencyAmount>();
                            var FeesList = veCore.Fees;

                            foreach (var item in FeesList)
                            {
                                StdCurrencyAmount stdCurrencyAmount = new StdCurrencyAmount
                                {
                                    Type = EnumCarCoverageType.MEDIUM, //usertodo
                                    //PayWhen =
                                    Desc = item.Description,
                                    Currency = item.CurrencyCode,
                                    Amount = item.Amount
                                };
                                stdAmount.Add(stdCurrencyAmount);
                            }
                            //押金添加进来
                            var payRule = ve.VehAvailInfo.PaymentRules;
                            if (payRule.Count > 0)
                            {
                                StdCurrencyAmount red = new StdCurrencyAmount
                                {
                                    Type = EnumCarCoverageType.Deposit,
                                    Desc = "",
                                    Currency = payRule.FirstOrDefault().CurrencyCode,
                                    Amount = payRule.FirstOrDefault().Amount,
                                    PayWhen = EnumCarPayWhen.Now,
                                };
                                stdAmount.Add(red);
                            }
                            //设备信息
                            var equips = veCore.PricedEquips;

                            List<StdPricedEquip> stdeq = new List<StdPricedEquip>();
                            foreach (var equip in equips)
                            {
                                var charge = equip.Charge;
                                StdPricedEquip stdPricedEquip = new StdPricedEquip
                                {
                                    IncludedInEstTotalInd = charge.IncludedInEstTotalInd,
                                    Currency = charge.CurrencyCode,
                                    UnitPrice = charge.Amount,
                                    MaxPrice = charge.Amount,
                                    TaxInclusive = charge.TaxInclusive,
                                    EquipDescription = charge.Description,
                                    EquipType = BuildEquipType(equip.Equipment.EquipType, charge.Description)
                                };
                                stdeq.Add(stdPricedEquip);
                            }
                            std.PricedEquips = stdeq;
                            //ratecode
                            std.RateCode = $"{EnumCarVendor.ACE_Rent_A_Car}_{veCore.Reference.Type}_{veCore.Reference.ID}";
                            //保险
                            List<StdPricedCoverage> stdPricedCoverages = new List<StdPricedCoverage>();
                            foreach (var pricedCoverage in veCore.PricedCoverages)
                            {
                                var charge = pricedCoverage.Charge;
                                var code = pricedCoverage.Coverage.Code;
                                StdPricedCoverage stdPriced = new StdPricedCoverage()
                                {
                                    CoverageType = BuildEnumCarCoverageType(pricedCoverage.Coverage.Code),
                                    CoverageDescription = charge.Description,
                                    Description = charge.Description,
                                    CurrencyCode = charge.CurrencyCode,
                                    Amount = charge.Amount,
                                    TaxInclusive = charge.TaxInclusive,
                                    IncludedInRate = charge.IncludedInRate,
                                    IncludedInEstTotalInd = charge.IncludedInEstTotalInd, //usertodo
                                    Calculation = new StdCalculation
                                    {
                                        Quantity = charge.Calculation.Quantity,
                                        UnitName = charge.Calculation.UnitName,
                                        UnitCharge = charge.Calculation.UnitCharge
                                    },
                                    // MaxCharge = Charge.MinMax.MaxCharge, //userloss
                                    //MinCharge = Charge.MinMax.MinCharge,  //userloss
                                };
                                stdPricedCoverages.Add(stdPriced);
                                if (stdPriced.CoverageType == EnumCarCoverageType.CollisionDamageWaiver)
                                {
                                    StdCurrencyAmount otherAmount = new StdCurrencyAmount
                                    {
                                        Type = EnumCarCoverageType.CollisionDamageWaiver,
                                        Desc = stdPriced.Description,
                                        Currency = stdPriced.CurrencyCode,
                                        Amount = stdPriced.Amount,
                                        PayWhen = EnumCarPayWhen.Now, //usertodo
                                    };
                                    stdAmount.Add(otherAmount);
                                }
                                if (stdPriced.CoverageType == EnumCarCoverageType.ThirdPartyLiability)
                                {
                                    StdCurrencyAmount otherAmount = new StdCurrencyAmount
                                    {
                                        Type = EnumCarCoverageType.ThirdPartyLiability,
                                        Desc = stdPriced.Description,
                                        Currency = stdPriced.CurrencyCode,
                                        Amount = stdPriced.Amount,
                                        PayWhen = EnumCarPayWhen.Now, //usertodo
                                    };
                                    stdAmount.Add(otherAmount);
                                }
                            }
                            std.PricedCoverages = stdPricedCoverages;
                            //其他金额 摘要
                            totalCharge.OtherAmounts = stdAmount;
                            std.TotalCharge = totalCharge;

                            var fuelPolicy = rentalRate.VehicleCharges.Where(n => n.Description.Contains("FUEL POLICY")).ToList();
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
                            var rateRule = await GetRateRule(veCore);
                            if (rateRule != null)
                            {
                                List<StdVehicleCharge> vehCharges = new List<StdVehicleCharge>();
                                var vehChargesList = rentalRate.VehicleCharges;
                                //费率

                                List<StdFee> stdfee = new List<StdFee>();
                                foreach (var fe in FeesList)
                                {
                                    var stdFee = _mapper.Map<Fee, StdFee>(fe);
                                    stdfee.Add(stdFee);
                                }
                                std.Fees = stdfee;
                                //车辆收费
                                var vehicleChargesList = vehChargesList.ToList();
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
                                        IncludedInEstTotalInd = item.IncludedInEstTotalInd,
                                        TaxInclusive = item.TaxInclusive,
                                    };
                                    if (item.Calculation != null)
                                    {
                                        var calculation = _mapper.Map<Calculation, StdCalculation>(item.Calculation);
                                        newItem.Calculation = calculation;
                                    }
                                    vehCharges.Add(newItem);
                                }
                                std.VehicleCharges = vehCharges;
                                //取车地点 还车地点
                                var loc = rateRule.LocationDetails.FirstOrDefault();
                                var startLoc = BuildLocation(loc);
                                var stdLoc = new StdLocation { PickUp = startLoc };
                                if (rateRule.LocationDetails.Count > 0)
                                {
                                    var endLoc = BuildLocation(rateRule.LocationDetails.LastOrDefault());
                                    stdLoc.DropOff = endLoc;
                                }
                                std.Location = stdLoc;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private StdLocationInfo BuildLocation(LocationDetails loc)
        {
            StdLocationInfo startLoc = new StdLocationInfo
            {
                //CounterLocation = BuildLocationType(), //usertodo
                LocationName = loc.Name,
                //LocationId=loc.Code, //locationid没有？
                CountryCode = loc.Address.CountryName.Code,
                CountryName = loc.Address.CountryName.Name,
                //StateProv= //省份没有
                CityName = loc.Address.CityName,
                Address = loc.Address.AddressLine,
                PostalCode = loc.Address.PostalCode,
                Telephone = loc.Telephones.FirstOrDefault().PhoneNumber,
                VendorLocId = loc.AdditionalInfo.ParkLocation.Location.ToString(), //停车位置 usertodo
                SuppLocId = loc.AdditionalInfo.CounterLocation.Location.ToString(),//柜台位置 usertodo
                ParkLocation = $"{loc.AdditionalInfo.ParkLocation.Location.ToString()}"
            };
            List<StdOperationTime> operationTimes = new List<StdOperationTime>();
            foreach (var date in loc.AdditionalInfo.OperationSchedules.OperationSchedule.OperationTimes)
            {
                operationTimes.Add(BuildOperationTime(date));
            }
            startLoc.OperationTimes = operationTimes;
            return startLoc;
        }

        /// <summary>
        /// 构建门店营业时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public StdOperationTime BuildOperationTime(OperationTime time)
        {
            var type = EnumCarWeek.Monday;
            if (time.Tue)
            {
                type = EnumCarWeek.Tuesday;
            }
            else if (time.Weds)
            {
                type = EnumCarWeek.Wednesday;
            }
            else if (time.Thur)
            {
                type = EnumCarWeek.Thursday;
            }
            else if (time.Fri)
            {
                type = EnumCarWeek.Friday;
            }
            else if (time.Sat)
            {
                type = EnumCarWeek.Saturday;
            }
            else if (time.Sun)
            {
                type = EnumCarWeek.Sunday;
            }
            return new StdOperationTime
            {
                Week = type,
                Times = new List<StdTime> { new StdTime { Start = time.Start, End = time.End } }
            };
        }

        /// <summary>
        /// 构建设备类型(不全)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        private EnumCarEquipType BuildEquipType(string type, string desc)
        {
            switch (type)
            {
                case "7":
                    return EnumCarEquipType.InfantSeat;

                case "8":
                    return EnumCarEquipType.ToddlerSeats;

                case "9":
                    return EnumCarEquipType.BoosterSeat;

                case "46":
                    return EnumCarEquipType.GPS;
            }
            Log.Information($"出现没有命中的设备类型{type}【{desc}】");
            return EnumCarEquipType.None;
        }

        /// <summary>
        /// 转化保险类型(不全)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public EnumCarCoverageType BuildEnumCarCoverageType(string code)
        {
            if (code.Contains("CDW"))
            {
                return EnumCarCoverageType.CollisionDamageWaiver;
            }
            else if (code.Contains("TPW"))
            {
                return EnumCarCoverageType.TheftProtection;
            }
            else if (code.Contains("3PI"))
            {
                return EnumCarCoverageType.ThirdPartyLiability;
            }
            else if (code.Contains("CCP"))
            {
                Log.Information($"CCP保险");
                return EnumCarCoverageType.BaseExcess;
            }
            Log.Information($"出现其他保险类型{code}");
            return EnumCarCoverageType.None;
        }

        /// <summary>
        /// 获取费率规则
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="veCore"></param>
        /// <returns></returns>
        public async Task<ACE_OTA_VehRateRuleRS> GetRateRule(VehAvailCore veCore)
        {
            var ruleRq = new ACE_OTA_VehRateRuleRQ
            {
                //返回请求数
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                Reference = veCore.Reference
            };
            var res = await BuildEnvelope(new CommonRequest { ACE_OTA_VehRateRuleRQ = ruleRq, Type = 2 });
            var model = ACEXmlHelper.GetResponse<ACE_OTA_VehRateRuleRS>(res);
            return model;
        }

        private EnumCarDoorCount BuildDoorCount(string str)
        {
            if (str.Contains("2") && str.Contains("4"))
            {
                return EnumCarDoorCount.Door2_4;
            }
            else if (str.Contains("4") && str.Contains("5"))
            {
                return EnumCarDoorCount.Door4_5;
            }
            else if (str.Contains("2") && str.Contains("3"))
            {
                return EnumCarDoorCount.Door2_3;
            }
            else if (str.Contains("2"))
            {
                return EnumCarDoorCount.Door2;
            }
            else if (str.Contains("3"))
            {
                return EnumCarDoorCount.Door3;
            }
            else if (str.Contains("4"))
            {
                return EnumCarDoorCount.Door4;
            }
            else if (str.Contains("5"))
            {
                return EnumCarDoorCount.Door5;
            }
            return EnumCarDoorCount.None;
        }

        public async Task<string> BuildEnvelope(CommonRequest model)
        {
            var body = new Body();
            switch (model.Type)
            {
                case 1:
                    body.OTA_VehAvailRateRQ = model.OTA_VehAvailRateRQ;
                    break;

                case 2:
                    body.ACE_OTA_VehRateRuleRQ = model.ACE_OTA_VehRateRuleRQ;
                    break;

                case 3:
                    body.ACE_OTA_VehResRQ = model.ACE_OTA_VehResRQ;
                    break;
            }
            var envelope = new ACEEnvelope
            {
                Body = body
            };
            return await ACEXmlHelper.PostRequest("http://ota.acerentacar.com/Sample/RateService.asmx", envelope);
        }

        /// <summary>
        /// 创建预订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ originModel, ACE_OTA_VehResRQ model, int timeout = 45000)
        {
            StdCreateOrderRS result = new StdCreateOrderRS();
            var res = await BuildEnvelope(new CommonRequest { ACE_OTA_VehResRQ = model, Type = 3 });
            var spModel = ACEXmlHelper.GetResponse<ACE_OTA_VehResRS>(res);
            if (spModel == null || spModel.Errors.Error)
            {
            }
        }

        #endregion 原始接口

        #region 业务接口

        public Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            //构建客户信息
            var customer = new Customer
            {
                Primary = new Primary
                {
                    PersonName = new PersonName
                    {
                        GivenName = createOrderRQ.FirstName,
                        //MiddleName=createOrderRQ.LastName, 中间名没有
                        Surname = createOrderRQ.LastName,
                    },
                    Telephone = new Models.RQs.Telephone
                    {
                        PhoneNumber = createOrderRQ.ContactNumber,
                        /* CountryAccessCode=createOrderRQ.BillingAddress.Country
                         AreaCityCode="",
                         Extension=""*/
                    },
                    Email = createOrderRQ.Email,
                    Address = new Models.RQs.Address
                    {
                        CityName = createOrderRQ.BillingAddress.CityName,
                        CountryName = createOrderRQ.BillingAddress.Country,
                        PostalCode = createOrderRQ.BillingAddress.PostCode,
                        AddressLine = createOrderRQ.BillingAddress.Address,
                        StreetNmbr = createOrderRQ.BillingAddress.Address
                    },
                    Document = null, //没有驾照信息
                    //BirthDate   =  //没有生日
                }
            };
            //解析一下 createOrderRQ.RateCode
            var ratecode = createOrderRQ.RateCode;
            var queryList = ratecode.Split("_");
            var id = queryList.LastOrDefault();
            var type = queryList[queryList.Length - 2];
            var availRateRQ = new ACE_OTA_VehResRQ
            {
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                VehResRQCore = new CreateOrder_VehResRQCore
                {
                    VehRentalCore = new VehRentalCore
                    {
                        PickUpDateTime = Convert.ToDateTime(createOrderRQ.PickUpDateTime),
                        ReturnDateTime = Convert.ToDateTime(createOrderRQ.ReturnDateTime),
                        //出发地 目的地方
                        /* PickUpLocation = new PickUpLocation { LocationCode = vehicleRQ.PickUpLocationCode },
                         ReturnLocation = new PickUpLocation { LocationCode = vehicleRQ.ReturnLocationCode }*/
                    },
                    Customer = customer
                },
                VehResRQInfo = new VehResRQInfo
                {
                    Reference = new Reference { ID = id, Type = type }
                }
            };
            throw new NotImplementedException();
        }

        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            //构建请求body 其他都一样
            var availRateRQ = new ACE_OTA_VehAvailRateRQ
            {
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                VehAvailRQCore = new VehAvailRQCore
                {
                    VehRentalCore = new Models.RQs.VehRentalCore
                    {
                        PickUpDateTime = Convert.ToDateTime(vehicleRQ.PickUpDateTime),
                        ReturnDateTime = Convert.ToDateTime(vehicleRQ.ReturnDateTime),
                        //出发地 目的地方
                        PickUpLocation = new PickUpLocation { LocationCode = vehicleRQ.PickUpLocationCode },
                        ReturnLocation = new PickUpLocation { LocationCode = vehicleRQ.ReturnLocationCode }
                    }
                }
            };
            return await VehAvailRate(availRateRQ);
        }

        public Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout)
        {
            throw new NotImplementedException();
        }

        #endregion 业务接口
    }
}