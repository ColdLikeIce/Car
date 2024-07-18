using Azure.Core;
using CommonCore.Mapper;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG.Models.Dtos;
using HeyTripCarWeb.Supplier.ACE.Util;
using HeyTripCarWeb.Supplier.BarginCar.Config;
using HeyTripCarWeb.Supplier.BarginCar.Model.Dbs;
using HeyTripCarWeb.Supplier.Sixt.Config;
using HeyTripCarWeb.Supplier.Sixt.Models.Dbs;
using HeyTripCarWeb.Supplier.Sixt.Models.RQs;
using HeyTripCarWeb.Supplier.Sixt.Models.RSs;
using HeyTripCarWeb.Supplier.Sixt.Util;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using Twilio.Jwt.AccessToken;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Public;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.Sixt
{
    /// <summary>
    /// 结算价和计佣价和代收都是retail rate。
    /// 结算价包含：Amount。
    /// 美元结算(月结)。
    ///
    ///
    /// 有押金  取消政策：取车前免费取消
    /// 目前第一步只实现到付
    /// 里程政策
    ///
    public class SixtApi : ISixtApi
    {
        private readonly SixtAppSetting _setting;
        private readonly IMapper _mapper;
        private readonly IRepository<CarLocationSupplier> _supplierCatRe;
        private readonly IRepository<CarCity> _CatCityRe;
        private readonly IRepository<SixtCountry> _countryRe;

        private readonly IRepository<SixtLocation> _locRepository;
        private readonly IRepository<SixtCarProReservation> _orderRepository;
        private readonly IServiceProvider _serviceProvider;

        public SixtApi(IOptions<SixtAppSetting> options, IMapper mapper, IRepository<CarLocationSupplier> supplierCatRe,
           IRepository<CarCity> catCityRe, IRepository<SixtLocation> locRepository,
           IRepository<SixtCarProReservation> orderRepository, IRepository<SixtCountry> countryRe, IServiceProvider serviceProvider)
        {
            _setting = options.Value;
            _mapper = mapper;
            _supplierCatRe = supplierCatRe;
            _CatCityRe = catCityRe;
            _countryRe = countryRe;
            _locRepository = locRepository;
            _orderRepository = orderRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task<List<CarLocationSupplier>> GetAllLocation()
        {
            var spLoc = SixtCacheInstance.Instance.GetAllItems();
            if (spLoc.Count == 0)
            {
                spLoc = await _supplierCatRe.GetListBySqlAsync("select * from CarRental.dbo.Car_Location_Suppliers where supplier =@supplier", new { supplier = (int)EnumCarSupplier.Sixt });
                SixtCacheInstance.Instance.SetLocation(spLoc);
            }
            return spLoc;
        }

        /// <summary>
        /// 构建门店地址
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BuildAllLocation()
        {
            var token = await SixtHttpHelper.GetToken(_setting);
            var (countryList, response) = await SixtHttpHelper.GetData<List<CountryRs>>($"{_setting.Url}/stations/countries", token, type: ApiEnum.SixtCountry);
            var existCountry = await _countryRe.GetAllAsync();
            var dbLoc = await _locRepository.GetAllAsync();
            var spLoc = await GetAllLocation();
            var allCity = await _CatCityRe.GetAllAsync();
            foreach (var country in countryList)
            {
                try
                {
                    var dbmodel = existCountry.FirstOrDefault(n => n.CountryCode == country.iso2code);
                    if (dbmodel == null)
                    {
                        dbmodel = new SixtCountry();
                    }
                    dbmodel.CountryName = country.name;
                    dbmodel.CountryCode = country.iso2code;
                    await _countryRe.InsertOrUpdate(dbmodel);

                    var (locList, response2) = await SixtHttpHelper.GetData<List<LocationRs>>($"{_setting.Url}/stations/country/{country.iso2code}?corporateCustomerNumber={_setting.CorporateCustomerNumber}", token, type: ApiEnum.Location);
                    if (locList == null)
                    {
                        continue;
                    }
                    foreach (var loc in locList)
                    {
                        try
                        {
                            SixtLocation location = new SixtLocation();
                            var exist = dbLoc.FirstOrDefault(n => n.VendorLocId == loc.Id);
                            if (exist != null)
                            {
                                location = exist;
                            }
                            location.LocationName = loc.Title;
                            location.VendorLocId = loc.Id;
                            location.CountryCode = country.iso2code;
                            location.CountryName = country.name;
                            location.CityName = loc.Address.City;
                            var city = allCity.FirstOrDefault(n => n.CityNameEn.ToLower() == location.CityName.ToLower());
                            if (city != null)
                            {
                                location.CityID = city.CityId;
                            }
                            location.Address = loc.Address?.Street;
                            location.Latitude = loc.Coordinates?.Latitude.ToString();
                            location.Longitude = loc.Coordinates?.Longitude.ToString();
                            location.Airport = string.IsNullOrWhiteSpace(loc.StationInformation.IataCode) ? false : true;
                            location.AirportCode = loc.StationInformation.IataCode;
                            location.PostalCode = loc.Address?.Postcode;
                            location.Telephone = loc.StationInformation.Contact?.Telephone;
                            location.Email = loc.StationInformation.Contact?.Email;
                            location.Supplier = (int)EnumCarSupplier.Sixt;
                            location.SuppLocId = loc.Id;
                            location.Vendor = (int)EnumCarVendor.Sixt;
                            location.VendorLocId = loc.Id;
                            if (exist == null)
                            {
                                location.CreateTime = DateTime.Now;
                            }
                            location.UpdateTime = DateTime.Now;

                            var openday = loc.StationInformation?.OpeningHours?.days;
                            if (openday != null)
                            {
                                List<StdOperationTime> all = new List<StdOperationTime>();
                                foreach (var item in openday)
                                {
                                    if (item.Key == "holidays")
                                    {
                                        continue;
                                    }
                                    List<StdTime> Times = new List<StdTime>();
                                    foreach (var te in item.Value.Openings)
                                    {
                                        Times.Add(new StdTime
                                        {
                                            Start = te.Open.Length > 5 ? te.Open.Substring(0, 5) : te.Open,
                                            End = te.Close.Length > 5 ? te.Close.Substring(0, 5) : te.Close,
                                        });
                                    }
                                    all.Add(new StdOperationTime
                                    {
                                        Week = EnumHelper.GetEnumTypeByStr<EnumCarWeek>(item.Key),
                                        Times = Times
                                    });
                                }
                                location.OperationTime = JsonConvert.SerializeObject(all);
                            }

                            await _locRepository.InsertOrUpdate(location);

                            //供应商标准表
                            var spModel = spLoc.FirstOrDefault(n => n.Supplier == location.Supplier.ToString() && n.VendorLocId == location.VendorLocId.ToString());
                            var isadd = false;
                            if (spModel == null)
                            {
                                spModel = new CarLocationSupplier();
                                isadd = true;
                            }
                            spModel.LocationName = location.LocationName;
                            spModel.CountryCode = location.CountryCode;
                            spModel.CountryName = location.CountryName;
                            spModel.CityName = location.CityName;
                            spModel.Address = location.Address;
                            spModel.Latitude = location.Latitude.ToString();
                            spModel.Longitude = location.Longitude.ToString();
                            spModel.Airport = string.IsNullOrWhiteSpace(location.AirportCode) ? false : true;
                            spModel.AirportCode = location.AirportCode;
                            spModel.StateCode = location.VendorLocId;
                            spModel.PostalCode = location.PostalCode;
                            spModel.Telephone = location.Telephone;
                            spModel.CityId = city == null ? 0 : city.CityId;
                            spModel.OperationTime = location.OperationTime;
                            spModel.Supplier = ((int)EnumCarSupplier.Jucy).ToString();
                            spModel.VendorLocId = location.VendorLocId.ToString();
                            spModel.SuppLocId = location.SuppLocId;
                            spModel.Supplier = location.Supplier.ToString();
                            spModel.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            if (isadd)
                            {
                                spModel.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            await _supplierCatRe.InsertOrUpdate(spModel);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"处理门店失败{ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"country处理失败{ex.Message}");
                }
            }
            SixtCacheInstance.Instance.SetLocation(new List<CarLocationSupplier>());
            return true;
        }

        public async Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            try
            {
                var dbOrder = await _orderRepository.GetByIdAsync(cancelOrderRQ.OrderNo);
                if (dbOrder == null)
                {
                    return new StdCancelOrderRS()
                    {
                        CancelSuc = false,
                        Message = $"没有找到相关的订单{cancelOrderRQ.OrderNo}"
                    };
                }
                var token = await SixtHttpHelper.GetToken(_setting);
                var url = $"{_setting.Url}/reservations/{dbOrder.ReservationId}";
                var (model, response) = await SixtHttpHelper.DeleteData<dynamic>(url, token, type: ApiEnum.Cancel);
                StdCancelOrderRS res = new StdCancelOrderRS();
                if (model == null)
                {
                    res.CancelSuc = false;
                }
                else if (model.errorCode != null && !string.IsNullOrWhiteSpace(model.errorCode.ToString()))
                {
                    res.CancelSuc = false;
                    res.Message = model.message;
                }
                else
                {
                    res.CancelSuc = true;

                    dbOrder.State = "canceled";
                    dbOrder.Status = model.status;
                    dbOrder.UpdateTime = DateTime.Now;
                    await _orderRepository.UpdateAsync(dbOrder);
                }
                return res;
            }
            catch (Exception ex)
            {
                return new StdCancelOrderRS
                {
                    CancelSuc = false,
                    Message = ex.Message,
                };
            }
        }

        private async Task<string> CreateOfferConfiguration(StdCreateOrderRQ createOrderRQ, string token, Dictionary<string, string> header)
        {
            var postModel = new
            {
                offerId = createOrderRQ.RateCode.Split("|").FirstOrDefault(),
                paymentOption = createOrderRQ.RateCode.Split("|").LastOrDefault()
            };
            var url = $"{_setting.Url}/offerConfiguration/create";
            var data = await SixtHttpHelper.PostData<dynamic>($"{url}", token, JsonConvert.SerializeObject(postModel), type: ApiEnum.SixtCreateConfig, headers: header);
            //当有装备的时候
            if (createOrderRQ.Extras != null && createOrderRQ.Extras.Count > 0)
            {
                List<object> eq = new List<object>();
                foreach (var extra in createOrderRQ.Extras)
                {
                    eq.Add(new
                    {
                        id = GetSupplierEquipType(extra.EquipType),
                        quantity = extra.Quantity,
                    });
                }
                url = $"{_setting.Url}/offerConfiguration/{data.id}";
                var postbody = new
                {
                    selections = eq,
                };
                await SixtHttpHelper.PostData<dynamic>($"{url}", token, JsonConvert.SerializeObject(postbody), headers: header, type: ApiEnum.SixtUpdateOfferConfig, method: "put");
            }
            return data.id;
        }

        public async Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            try
            {
                var dbOrder = await _orderRepository.GetByIdAsync(createOrderRQ.OrderNo);
                if (dbOrder != null)
                {
                    return new StdCreateOrderRS()
                    {
                        OrderSuc = false,
                        SuppOrderId = dbOrder.ReservationId,
                        Message = $"已存在订单{dbOrder.OrderNo}"
                    };
                }
                StdCreateOrderRS res = new StdCreateOrderRS();

                var token = await SixtHttpHelper.GetToken(_setting);
                var header = new Dictionary<string, string>();
                header.Add("Currency", "EUR");
                //创建优惠配置
                var configid = await CreateOfferConfiguration(createOrderRQ, token, header);
                var url = $"{_setting.Url}/reservations";
                var birday = "";
                if (createOrderRQ.DriverAge > 0)
                {
                    birday = DateTime.Now.AddYears(-createOrderRQ.DriverAge).ToString("yyyy-MM-dd");
                }
                var countryCode = createOrderRQ.ContactNumber.Split("_").FirstOrDefault();
                if (countryCode.Length > 4)
                {
                    countryCode = "";
                }
                var postdata = new RentalBookingRQ
                {
                    ConfigurationId = configid,
                    FlightNumber = createOrderRQ.FlightNumber,
                    Drivers = new List<Driver>
                    {
                        new Driver{
                            GivenName=createOrderRQ.FirstName,
                            FamilyName = createOrderRQ.LastName,
                            Birthdate = birday,
                            Contact = new ContactDetails
                            {
                                Email=createOrderRQ.Email,
                                Telephone= new Telephone{
                                CountryCode = countryCode,
                                Number =createOrderRQ.ContactNumber
                                },
                            },
                        }
                    },
                    BrokerEmail = createOrderRQ.Email,
                    AgencyNumber = _setting.AgencyNumber
                };
                var spmodel = await SixtHttpHelper.PostData<ReservationRs>($"{url}", token, JsonConvert.SerializeObject(postdata), type: ApiEnum.Create, headers: header);

                if (!string.IsNullOrWhiteSpace(spmodel.errorCode))
                {
                    res.OrderSuc = false;
                    res.Message = spmodel.message;
                    return res;
                }
                else
                {
                    SixtCarProReservation order = new SixtCarProReservation
                    {
                        OrderNo = createOrderRQ.OrderNo,
                        ReservationId = spmodel.Id,
                        DisplayId = spmodel.DisplayId,
                        State = spmodel.State,
                        Status = spmodel.Status,
                        PointOfSale = spmodel.PointOfSale,
                        CreateDate = spmodel.CreateDate,
                        Vehicle = spmodel.Vehicle != null ? JsonConvert.SerializeObject(spmodel.Vehicle) : "",
                        Drivers = spmodel.Drivers != null ? JsonConvert.SerializeObject(spmodel.Drivers) : "",
                        PickupDate = spmodel.PickupDate,
                        PickupStation = spmodel.PickupStation != null ? JsonConvert.SerializeObject(spmodel.PickupStation) : "",
                        ReturnDate = spmodel.ReturnDate,
                        ReturnStation = spmodel.ReturnStation != null ? JsonConvert.SerializeObject(spmodel.ReturnStation) : "",
                        Delivery = spmodel.Delivery != null ? JsonConvert.SerializeObject(spmodel.Delivery) : "",
                        Collection = spmodel.Collection != null ? JsonConvert.SerializeObject(spmodel.Collection) : "",
                        OfferConfiguration = spmodel.OfferConfiguration != null ? JsonConvert.SerializeObject(spmodel.OfferConfiguration) : "",
                        SelfService = spmodel.SelfService != null ? JsonConvert.SerializeObject(spmodel.SelfService) : "",
                        ReferenceFields = spmodel.ReferenceFields != null ? JsonConvert.SerializeObject(spmodel.ReferenceFields) : "",
                        BonusProgram = spmodel.BonusProgram != null ? JsonConvert.SerializeObject(spmodel.BonusProgram) : "",
                        CustomerType = spmodel.CustomerType,
                        CorporateCustomerNumber = spmodel.CorporateCustomerNumber,
                        RateCode = createOrderRQ.RateCode,
                        CreateTime = DateTime.Now
                    };
                    await _orderRepository.InsertAsync(order);
                    res.OrderSuc = true;
                    res.SuppOrderId = spmodel.Id;
                    res.SuppOrderStatus = spmodel.Status;
                    //res.SuppConfirmNumber =
                    res.SuppCurrency = spmodel.OfferConfiguration.Prices.TotalPrice.Currency;
                    res.SuppTotalPrice = spmodel.OfferConfiguration.Prices.TotalPrice.Amount;
                    res.SuppConfirmed = true;
                }
                return res;
            }
            catch (Exception ex)
            {
                return new StdCreateOrderRS
                {
                    OrderSuc = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<List<StdVehicle>> GetVehiclesByloc(QueryDto dto)
        {
            List<StdVehicle> stdVehicles = new List<StdVehicle>();
            var token = dto.SixtToken;
            var url = $"{_setting.Url}/offers?pickupStationId={dto.PickUpLocationCode}&returnStationId={dto.ReturnLocationCode}&pickupDate={dto.PickUpDateTime.ToString("yyyy-MM-ddTHH:mm:ss")}&returnDate={dto.ReturnDateTime.ToString("yyyy-MM-ddTHH:mm:ss")}";
            if (dto.DriverAge > 0)
            {
                url += $"&driverAge={dto.DriverAge}";
            }
            if (!string.IsNullOrWhiteSpace(_setting.filter))
            {
                url += $"&{_setting.filter}";
            }
            url += $"&pointOfSale={dto.startLoc.CountryCode}&corporateCustomerNumber={_setting.CorporateCustomerNumber}";
            var header = new Dictionary<string, string>();
            header.Add("Currency", "EUR");
            //header.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
            var (stdList, response) = await SixtHttpHelper.GetData<QueryRs>($"{url}", token, headers: header, type: ApiEnum.List);
            if (stdList == null)
            {
                return stdVehicles;
            }
            if (!string.IsNullOrWhiteSpace(stdList.errorcode))
            {
                Log.Information($"{stdList.errorcode}");
                return stdVehicles;
            }
            var termsAndConditionsRef = stdList.documents.TermsAndConditions;
            foreach (var of in stdList.offers)
            {
                try
                {
                    var sipp = of.VehicleGroupInfo?.GroupInfo?.VehicleGroup;
                    StdVehicle std = new StdVehicle
                    {
                        Supplier = EnumCarSupplier.Sixt,
                        Vendor = EnumCarVendor.Sixt,
                        VehicleName = of.Title,
                        VehicleGroup = (EnumCarVehicleGroup)SIPPHelper.SIPPCodeAnalysis(sipp, 3),
                        ProductCode = $"{EnumCarSupplier.Sixt}_{dto.startLoc.VendorLocId}_{sipp}",
                        VehicleCode = sipp,
                        DoorCount = (EnumCarDoorCount)of.VehicleGroupInfo?.GroupInfo?.Doors,
                        VehicleCategory = EnumHelper.GetEnumTypeByStr<EnumCarVehicleCategory>(of.VehicleGroupInfo?.Type),
                        VehicleClass = (EnumCarVehicleClass)SIPPHelper.SIPPCodeAnalysis(sipp, 4), //usertodo
                        AirConditioning = of.VehicleGroupInfo.GroupInfo.HasAirConditioning,
                        DriveType = EnumCarDriveType.Unspecified, //usertodo
                        FuelType = of.VehicleGroupInfo.GroupInfo.IsElectric ? EnumCarFuelType.Electric : EnumCarFuelType.Unspecified,
                        TransmissionType = of.VehicleGroupInfo.GroupInfo.IsAutomatic ? EnumCarTransmissionType.Automatic : EnumCarTransmissionType.Manual,
                        FuelPolicy = EnumCarFuelPolicy.None, //usertodo 燃油政策
                        PassengerQuantity = of.VehicleGroupInfo.GroupInfo.MaxPassengers,
                        BaggageQuantity = of.VehicleGroupInfo.GroupInfo.Baggage.Bags,
                        PictureURL = of.VehicleGroupInfo.GroupInfo.ImageUrl,
                        MinDriverAge = of.VehicleGroupInfo.GroupInfo.DriverRequirements.MinAge,
                        DriverInfo = new StdDriverInfo
                        {
                            YoungDriverMinAge = of.VehicleGroupInfo.GroupInfo.DriverRequirements.MinAge,
                            MinDrivingExperience = of.VehicleGroupInfo.GroupInfo.DriverRequirements.LicenseMinYears
                        },
                        //TermsAndConditionsRef = termsAndConditionsRef
                    };

                    //里程限制
                    foreach (var rate in of.MileagePlans)
                    {
                        if (rate.Selected)
                        {
                            var ratedistince = new StdRateDistance
                            {
                                Unlimited = rate.Unlimited,
                                Description = rate.Title
                            };
                            if (rate.IncludedMileage != null)
                            {
                                ratedistince.Quantity = rate.IncludedMileage.Value;
                                ratedistince.DistUnitName = rate.IncludedMileage.Unit == "mi" ? EnumCarDistUnitName.Mile : EnumHelper.GetEnumTypeByStr<EnumCarDistUnitName>(rate.IncludedMileage.Unit);
                                ratedistince.Currency = rate.AdditionalMileage.Currency;
                                ratedistince.Amount = rate.AdditionalMileage.Price;
                                ratedistince.VehiclePeriodUnitName = EnumCarPeriodUnitName.RentalPeriod; //usertodo
                            }
                            std.RateDistance = ratedistince;
                            break;
                        }
                    }
                    //价格
                    StdTotalCharge stdTotal = new StdTotalCharge();
                    stdTotal.PriceType = EnumCarPriceType.RetailRate; //usertodo
                    var payitem = of.Payment.PaymentOptions.FirstOrDefault(n => n.Id == of.Payment.SelectedPaymentOption);
                    stdTotal.Currency = payitem.Price.Currency;
                    stdTotal.TotalAmount = payitem.Price.Amount; //usertodo 有个net  预付款需要佣金
                    stdTotal.RentalAmount = of.Prices.BasePrice.Amount; //当当租车金

                    if (of.Payment.SelectedPaymentOption == "pay_on_arrival")
                    {
                        stdTotal.PayType = EnumCarPayType.PayOnArrival;
                    }
                    else if (of.Payment.SelectedPaymentOption == "prepaid")
                    {
                        Log.Information($"出现预付款【{JsonConvert.SerializeObject(dto)}】");
                        stdTotal.PayType = EnumCarPayType.Prepaid;
                    }
                    else
                    {
                        Log.Error($"usertodel存在其他支付类型{of.Payment.SelectedPaymentOption}");
                    }
                    //取消政策 免费取消
                    std.CancelPolicy = new StdCancelPolicy
                    {
                        CancelType = EnumCarCancelType.FeeCancel,
                        Rules = new List<StdCancelRule>
                        {
                            new StdCancelRule
                            {
                                DeductType = EnumCarDeductType.Free,
                                Currency =  stdTotal.Currency,
                                StartTime = DateTime.Now,
                                EndTime = dto.PickUpDateTime,
                                DeductValue = 0,
                            }
                        }
                    };
                    std.TotalCharge = stdTotal;
                    //押金信息
                    List<StdPricedCoverage> pricedCoverageList = new List<StdPricedCoverage>();
                    if (of.DepositAmount != null)
                    {
                        stdTotal.AddOtherAmount(EnumCarCoverageType.Deposit, EnumCarPayWhen.NoNeed, of.DepositCurrency, of.DepositAmount.Value);
                    }
                    var protectList = of.Charges.Where(n => n.Type == "protection").ToList();
                    List<string> containStatus = new List<string> { "selected", "included", "mandatory" };
                    foreach (var pro in protectList)
                    {
                        /*if (pro.Status != "available" && pro.Id != "LD" && pro.Id != "BF")
                        {
                            Log.Information($"存在保险类型{pro.Status}");
                        }*/
                        StdPricedCoverage pricedCoverage = new StdPricedCoverage
                        {
                            Required = pro.Status == "mandatory" ? true : false,
                            CoverageType = BuildEnumCarCoverageType(pro.Id),
                            CoverageDescription = pro.Description,
                            CurrencyCode = pro.Price.Currency,
                            Amount = pro.Price.Amount,
                            MinCharge = pro.ExcessAmount?.Value
                        };

                        if (pricedCoverage.CoverageType == EnumCarCoverageType.None && !exceptProtect.Contains(pro.Id))
                        {
                            Log.Information($"usertodel存在不同的保险类型{JsonConvert.SerializeObject(pro)}");
                        }
                        if (containStatus.Contains(pro.Status))
                        {
                            pricedCoverage.IncludedInEstTotalInd = true;
                        }
                        pricedCoverage.Calculation = new StdCalculation
                        {
                            UnitCharge = pro.Price.Amount,
                            UnitName = pro.Price.Unit,
                            Quantity = 1
                        };
                        pricedCoverageList.Add(pricedCoverage);

                        var included = containStatus.Contains(pro.Status);//包含到总价
                        if (included) //包含了才输出
                        {
                            std.TotalCharge.AddOtherAmount(pricedCoverage.CoverageType, EnumCarPayWhen.NoNeed, pro.Price.Currency, pro.Price.Amount);
                        }
                    }
                    std.PricedCoverages = pricedCoverageList;
                    //设备信息
                    var othereq = of.Charges.Where(n => n.Type == "other").ToList();
                    List<StdPricedEquip> equips = new List<StdPricedEquip>();
                    foreach (var eq in othereq)
                    {
                        var equipType = BuildEnumCarEquipType(eq);
                        if (equipType == EnumCarEquipType.None && !ExceptCode.Contains(eq.Id))
                        {
                            Log.Information($"usertodel:存在没有处理的other类型【{JsonConvert.SerializeObject(eq)}】");
                        }
                        if (equipType == EnumCarEquipType.None)
                        {
                            continue;
                        }
                        StdPricedEquip pricedEquip = new StdPricedEquip
                        {
                            EquipType = equipType,
                            EquipDescription = eq.Description,
                            Unit = BuildequipUnitType(eq.Price.Unit),
                            Currency = eq.Price.Currency,
                            UnitPrice = eq.Price.Amount,
                            MaxQuantity = eq.MaxQuantity,
                        };
                        if (containStatus.Contains(eq.Status))
                        {
                            pricedEquip.IncludedInEstTotalInd = true;
                        }
                        if (pricedEquip.EquipType == EnumCarEquipType.None)
                        {
                            pricedEquip.EquipDescription = $"{eq.Id}|{eq.Description}";
                        }
                        equips.Add(pricedEquip);
                    }
                    std.PricedEquips = equips;
                    List<StdVehicleCharge> charges = new List<StdVehicleCharge>();
                    //押金
                    StdVehicleCharge deposit = new StdVehicleCharge
                    {
                        Purpose = EnumCarCoverageType.Deposit,
                        Description = "Deposit",
                        CurrencyCode = of.DepositCurrency,
                        Amount = of.DepositAmount == null ? 0 : of.DepositAmount.Value,
                    };
                    charges.Add(deposit);
                    //原始供应商价格
                    StdVehicleCharge stdVehicle = new StdVehicleCharge
                    {
                        Purpose = EnumCarCoverageType.VehicleRentalAmount,
                        Description = "VehicleRentalAmount",
                        CurrencyCode = of.Prices.TotalPrice.Currency,
                        Amount = of.Prices.TotalPrice.Amount,
                        TaxInclusive = true
                    };
                    charges.Add(stdVehicle);
                    //年轻驾驶员价格
                    if (of.Prices.YoungDriverChargeDayPrice != null)
                    {
                        std.DriverInfo.YoungDriverFee = of.Prices.YoungDriverChargeDayPrice.Gross;
                        std.DriverInfo.YoungDriverFeeUnit = EnumHelper.GetEnumTypeByStr<EnumCarPeriodUnitName>(of.Prices.YoungDriverChargeDayPrice.Unit);
                        StdVehicleCharge young = new StdVehicleCharge
                        {
                            Purpose = EnumCarCoverageType.YoungDriverSurcharge,
                            Description = "YoungDriverSurcharge",
                            CurrencyCode = of.Prices.YoungDriverChargeDayPrice.Currency,
                            Amount = of.Prices.YoungDriverChargeDayPrice.Gross,
                            TaxInclusive = true,
                            /*  Calculation = new StdCalculation
                              {
                                  UnitCharge = of.Prices.YoungDriverChargeDayPrice.Gross,
                                  UnitName = of.Prices.YoungDriverChargeDayPrice.UnitKey,
                                  Quantity = 1
                              }*/
                        };
                        charges.Add(young);
                    }
                    std.VehicleCharges = charges;
                    std.RateCode = $"{of.Id}|{of.Payment.SelectedPaymentOption}";
                    std.Vendor = EnumCarVendor.Sixt;
                    std.VendorName = EnumCarVendor.Sixt.ToString();
                    std.VendorLogo = "todo";
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
                            OperationTimes = JsonConvert.DeserializeObject<List<StdOperationTime>>(pickUp.OperationTime)
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
                            OperationTimes = JsonConvert.DeserializeObject<List<StdOperationTime>>(endLoc.OperationTime)
                        }
                    };
                    stdVehicles.Add(std);
                }
                catch (Exception ex)
                {
                    Log.Error($"构建{JsonConvert.SerializeObject(of)}失败{ex.Message}");
                }
            }

            return stdVehicles;
        }

        private List<string> ExceptCode = new List<string>
        {
            "LS","4W","LR","FF","TR","I3","EZ","SV","I4","T1","TF","NG","UF","P1","T4","T2","IE","CM",
            "UF","P1","CK","DV","LN","AU","TL","L0","LE","UG","AC","P1","P1","MC","E","PV","M1","LT","D8","OF","HT","C2","VA",
            "DA",""
        };

        private Dictionary<string, EnumCarEquipType> dict = new Dictionary<string, EnumCarEquipType>()
        {
            {"CS",EnumCarEquipType.ChildSeat },
            {"AD",EnumCarEquipType.AdditionalDriver },
            {"BT",EnumCarEquipType.BlueTooth },
            {"L",EnumCarEquipType.STR },
            {"SR",EnumCarEquipType.SKR },
            //{"NG",EnumCarEquipType.GPS },
            {"BO",EnumCarEquipType.BoosterSeat },
            {"BS",EnumCarEquipType.InfantSeat },
            {"SC",EnumCarEquipType.SnowChains },
            {"DI",EnumCarEquipType.DSL},
            //{"UF",EnumCarEquipType.CBF },
            {"FU",EnumCarEquipType.CBF },
            {"NV",EnumCarEquipType.GPS},
            {"I2",EnumCarEquipType.WIFI },
            {"SK",EnumCarEquipType.Ski_Box },
            {"BB",EnumCarEquipType.Baby_Stroller },
            {"L9",EnumCarEquipType.WinterPackage }
        };

        /// <summary>
        /// 构建供应商那边设备枚举
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetSupplierEquipType(EnumCarEquipType type, string remark = "")
        {
            /*if (type == EnumCarEquipType.None)
            {
                return remark.Split("|").FirstOrDefault();
            }*/
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
        /// 构建设备枚举
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EnumCarEquipType BuildEnumCarEquipType(ChargeItem item)
        {
            var id = item.Id;
            if (dict.TryGetValue(id, out EnumCarEquipType value))
            {
                return value;
            }
            else
            {
                return EnumCarEquipType.None; //跨国*/
            }
            /*   switch (id)
               {
                   case "CS":
                       return EnumCarEquipType.ChildSeat;

                   case "AD":
                       return EnumCarEquipType.AdditionalDriver;

                   case "CM":
                       return EnumCarEquipType.BlueTooth;

                   case "L":
                       return EnumCarEquipType.STR;

                   case "SR":
                       return EnumCarEquipType.SKR;

                   case "NG":
                       return EnumCarEquipType.GPS;

                   case "LS":
                       return EnumCarEquipType.None; //袋子
                   case "BO":
                       return EnumCarEquipType.BoosterSeat;

                   case "BS":
                       return EnumCarEquipType.InfantSeat;
                   *//*    case "FF":
                           return //燃油政策*//*
                   case "SC":
                       return EnumCarEquipType.SnowChains;

                   case "UF":  //跨国
                   case "LR":  //固定装置
                       return EnumCarEquipType.None; //跨国
               }
               Log.Information($"usertodel:存在没有处理的other类型{id}【{item.Description}】");
               return EnumCarEquipType.None; //跨国*/
        }

        /// <summary>
        /// 构建设备单位枚举
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EnumCarPeriodUnitName BuildequipUnitType(string id)
        {
            switch (id)
            {
                case "day/unit":
                case "day":
                case "day/driver":
                    return EnumCarPeriodUnitName.Day;

                case "price/unit":
                case "total":
                case "One-time":
                    return EnumCarPeriodUnitName.RentalPeriod;
            }
            Log.Information($"存在其他设备单位类型{id}");
            return EnumCarPeriodUnitName.RentalPeriod;
        }

        private List<string> exceptProtect = new List<string>
        {
            "BQ",
            "GP", //碎石保险
            "SA", //沙尘保护
            "BR",  //内部包含
            "H3",  //提前预定
        };

        /// <summary>
        /// 构建保险类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public EnumCarCoverageType BuildEnumCarCoverageType(string id)
        {
            switch (id)
            {
                case "LD":
                case "BE":
                case "BF":
                    return EnumCarCoverageType.LossDamageWaiver;

                case "TG":
                    return EnumCarCoverageType.WWI;

                case "BC":
                    return EnumCarCoverageType.RSA;

                case "I":
                    return EnumCarCoverageType.PersonalAccidentInsurance;

                case "IP":
                case "J":
                    return EnumCarCoverageType.PersonalEffectsCoverage;

                case "V":
                case "G":
                case "GF":
                case "coverage-base-none":
                    return EnumCarCoverageType.CollisionDamageWaiver;

                case "D":
                case "H":
                    return EnumCarCoverageType.TheftProtection;

                case "GT":
                    return EnumCarCoverageType.WWI;

                case "SL":
                case "S3":
                    return EnumCarCoverageType.SupplementaryLiabilityInsurance;

                case "LB":
                    return EnumCarCoverageType.LossDamageWaiver;
            }

            return EnumCarCoverageType.None;
        }

        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            Log.Information($"接收到参数{JsonConvert.SerializeObject(vehicleRQ)}");
            List<StdVehicle> firstList = new List<StdVehicle>();
            try
            {
                var locList = await GetAllLocation();
                var (startLocList, endLocList) = await CommonLocationHelper.GetLocaiton(vehicleRQ, locList);
                int batchSize = 4; // 限制 10个任务跑
                List<Task> runTasks = new List<Task>();
                var token = await SixtHttpHelper.GetToken(_setting);
                //43686
                //startLocList = startLocList.Where(n => n.SuppLocId == "44624").ToList();
                foreach (var start in startLocList)
                {
                    if (vehicleRQ.PickUpLocationCode == vehicleRQ.ReturnLocationCode)
                    {
                        QueryDto queryDto = new QueryDto
                        {
                            PickUpDateTime = vehicleRQ.PickUpDateTime,
                            ReturnDateTime = vehicleRQ.ReturnDateTime,
                            PickUpLocationCode = start.VendorLocId,
                            ReturnLocationCode = start.VendorLocId,
                            DriverAge = vehicleRQ.DriverAge,
                            CitizenCountryCode = vehicleRQ.CitizenCountryCode,
                            startLoc = start,
                            endLoc = start,
                            SixtToken = token
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
                        foreach (var end in endLocList)
                        {
                            QueryDto queryDto = new QueryDto
                            {
                                PickUpDateTime = vehicleRQ.PickUpDateTime,
                                ReturnDateTime = vehicleRQ.ReturnDateTime,
                                PickUpLocationCode = start.VendorLocId,
                                ReturnLocationCode = start.VendorLocId,
                                DriverAge = vehicleRQ.DriverAge,
                                CitizenCountryCode = vehicleRQ.CitizenCountryCode,
                                startLoc = start,
                                endLoc = end
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
                    }
                }
                await Task.WhenAll(runTasks);
                var ss = JsonConvert.SerializeObject(firstList);
                Log.Information($"return:【{firstList.Count}】");
                return firstList;
            }
            catch (Exception ex)
            {
                Log.Error($"查询出现异常{ex.Message}");
                return firstList;
            }
        }

        public async Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout = 15000)
        {
            try
            {
                StdQueryOrderRS result = new StdQueryOrderRS();
                var dbOrder = await _orderRepository.GetByIdAsync(queryOrderRQ.OrderNo);
                if (dbOrder == null)
                {
                    return new StdQueryOrderRS()
                    {
                        IsSuccess = false,
                        SuppOrderId = dbOrder.ReservationId,
                        ErrorMessage = $"没有找到相关的订单{queryOrderRQ.OrderNo}"
                    };
                }
                var token = await SixtHttpHelper.GetToken(_setting);
                var header = new Dictionary<string, string>();
                header.Add("Currency", "EUR");
                //创建优惠配置
                var url = $"{_setting.Url}/reservations/{dbOrder.ReservationId}";

                var (spmodel, response) = await SixtHttpHelper.GetData<ReservationRs>($"{url}", token, headers: header, type: ApiEnum.Detail);
                if (spmodel == null)
                {
                    result.IsSuccess = false;
                }
                else if (!string.IsNullOrWhiteSpace(spmodel.errorCode))
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = spmodel.message;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Status = spmodel.State == "cancelled" ? EnumCarReservationStatus.Cancelled : EnumCarReservationStatus.Confirmed;
                    result.SuppOrderId = spmodel.Id;
                    result.Currency = spmodel.OfferConfiguration.Prices.TotalPrice.Currency;
                    result.Amount = spmodel.OfferConfiguration.Prices.TotalPrice.Amount;
                    result.SIPP = spmodel.OfferConfiguration?.VehicleGroup;
                }
                result.OrigData = response;
                return result;
            }
            catch (Exception ex)
            {
                return new StdQueryOrderRS
                {
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task test()
        {
            var locList = await GetAllLocation();
            List<Task> runTasks = new List<Task>();
            int batchSize = 10; // 限制 10个任务跑
            List<StdVehicle> res = new List<StdVehicle>();
            var token = await SixtHttpHelper.GetToken(_setting);
            foreach (var start in locList)
            {
                try
                {
                    QueryDto queryDto = new QueryDto
                    {
                        PickUpDateTime = DateTime.Now.AddMonths(2),
                        ReturnDateTime = DateTime.Now.AddDays(1).AddMonths(2),
                        PickUpLocationCode = start.VendorLocId,
                        ReturnLocationCode = start.VendorLocId,
                        CitizenCountryCode = "US",
                        startLoc = start,
                        endLoc = start,
                        SixtToken = token
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
                }
                catch (Exception ex)
                {
                    Log.Error($"任务执行出错{ex.Message}");
                }
            }
            await Task.WhenAll(runTasks);
            Log.Information($"任务结束");
        }
    }
}