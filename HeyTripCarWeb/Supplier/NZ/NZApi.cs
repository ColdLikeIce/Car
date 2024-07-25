using CommonCore.EntityFramework.Common;
using CommonCore.Mapper;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ACE.Util;
using HeyTripCarWeb.Supplier.BarginCar.Config;
using HeyTripCarWeb.Supplier.BarginCar.Model.Dbs;
using HeyTripCarWeb.Supplier.BarginCar.Model.Dtos;
using HeyTripCarWeb.Supplier.BarginCar.Model.RQs;
using HeyTripCarWeb.Supplier.BarginCar.Model.RSs;
using HeyTripCarWeb.Supplier.BarginCar.Util;
using HeyTripCarWeb.Supplier.NZ.Config;
using HeyTripCarWeb.Supplier.NZ.Model.Dbs;
using HeyTripCarWeb.Supplier.NZ.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.NZ
{
    public class NZApi : INZApi
    {
        private readonly NZCarAppSetting _setting;
        private readonly IMapper _mapper;

        private readonly IBaseRepository<CarRentalDbContext> _cr_repository;
        private readonly IBaseRepository<CarSupplierDbContext> _repository;

        private static EnumCarVendor _vendor = EnumCarVendor.Bargain_Car_Rentals;

        public NZApi(IOptions<NZCarAppSetting> options, IMapper mapper, IBaseRepository<CarRentalDbContext> cr_repository,
            IBaseRepository<CarSupplierDbContext> repository)
        {
            _setting = options.Value;
            _mapper = mapper;
            _cr_repository = cr_repository;
            _repository = repository;
        }

        private int suppiler = 26; //todo 后续改成

        private Dictionary<int, NZLocation> locationMap = new Dictionary<int, NZLocation>()
        {
            {9,new NZLocation{ CountryCode ="NZ",CountryName="New Zealand",Latitude="-36.97971",Longitude="174.773749",CityID=1254,CityName="Auckland",AirportCode="AKL",Airport=true} },
            {17, new NZLocation{ CountryCode = "NZ", CountryName = "New Zealand", Latitude = "-36.84674", Longitude = "174.77017", CityID = 1254, CityName = "Auckland",AirportCode="AKL",Airport=true} },
            {15, new NZLocation{CountryCode = "NZ",CountryName = "New Zealand",Latitude = "-43.4861",Longitude = "172.5291",CityID = 1256,CityName = "Christchurch",AirportCode = "CHC",Airport = true}},
            {22, new NZLocation{ CountryCode = "NZ", CountryName = "New Zealand", Latitude = "-45.0312", Longitude = "168.6626", CityID =1270 , CityName = "Queenstown", AirportCode = "", Airport = false }},
            {20, new NZLocation{ CountryCode = "NZ", CountryName = "New Zealand", Latitude = "-41.3223", Longitude = "174.8050", CityID = 1275, CityName = "Wellington", AirportCode = "WLG", Airport = true }},
        };

        public string BuildOperationTime(List<OfficeTime> timeList, int locationId)
        {
            List<StdOperationTime> all = new List<StdOperationTime>();
            var loctime = timeList.Where(n => n.LocationId == locationId).ToList();
            foreach (EnumCarWeek log in Enum.GetValues(typeof(EnumCarWeek)))
            {
                all.Add(new StdOperationTime
                {
                    Week = log,
                    Times = new List<StdTime> { new StdTime {
                    Start = loctime.FirstOrDefault(n => n.DayOfWeek == (int)log).OpeningTime,
                    End = loctime.FirstOrDefault(n => n.DayOfWeek == (int)log).ClosingTime,
                }
                }
                });
            }
            return JsonConvert.SerializeObject(all);
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
        /// 预订车辆
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<BarginBookingResponse> VehRes(StdCreateOrderRQ createOrderRQ, string[] extensionsInfo)
        {
            try
            {
                var ageid = await GetSupplierAgeId(createOrderRQ.DriverAge);
                if (extensionsInfo != null && extensionsInfo.Length > 0)
                {
                    BarginBookingRequest request = new BarginBookingRequest
                    {
                        vehiclecategorytypeid = extensionsInfo[0],
                        pickuplocationid = extensionsInfo[1],
                        pickupdate = createOrderRQ.PickUpDateTime.ToString("dd/MM/yyyy"),
                        pickuptime = createOrderRQ.PickUpDateTime.ToString("HH:mm"),
                        dropofflocationid = extensionsInfo[2],
                        dropoffdate = createOrderRQ.ReturnDateTime.ToString("dd/MM/yyyy"),
                        dropofftime = createOrderRQ.ReturnDateTime.ToString("HH:mm"),
                        ageid = ageid,
                        vehiclecategoryid = extensionsInfo[3],
                        insuranceid = extensionsInfo[4],
                        extrakmsid = extensionsInfo[5],
                        transmission = "0",//Transmission preference: 0=no preference, 1=auto, 2=manual
                        bookingtype = "2",//Booking type (1=quote, 2=booking)
                        method = "booking",
                        numbertravelling = "1",
                        customer = new BarginCustomer
                        {
                            firstname = createOrderRQ.FirstName,
                            lastname = createOrderRQ.LastName,
                            email = createOrderRQ.Email,
                            dateofbirth = "",
                            address = createOrderRQ.BillingAddress.Address,
                            city = createOrderRQ.BillingAddress.CityName,
                            licenseno = "",
                            postcode = createOrderRQ.BillingAddress.PostCode,
                            state = ""
                        }
                    };
                    if (createOrderRQ.Extras.Count > 0)
                    {
                        //request.optionalfees =
                        List<optionalfees> eqList = new List<optionalfees>();
                        foreach (var item in createOrderRQ.Extras)
                        {
                            var id = GetSupplierEquipType(item.EquipType);
                            eqList.Add(new optionalfees
                            {
                                id = Convert.ToInt32(id),
                                qty = item.Quantity,
                            });
                        }
                        request.optionalfees = eqList;
                    }
                    var postData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    var (response, rs) = await NZHttpHelper.BasePostRequest<BarginBookingResponse>(postData, _setting, ApiEnum.Create);
                    if (response != null && response.status != "OK" && !string.IsNullOrEmpty(response.error))
                    {
                        Log.Error(response.error);
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return new BarginBookingResponse();
        }

        /// <summary>
        /// 预定查询
        /// </summary>
        /// <returns></returns>
        public async Task<(BarginBookingInfoRoot, string)> VehRetRes(string suppOrderId)
        {
            var postData = $"{{\"method\":\"bookinginfo\",\"reservationref\":\"{suppOrderId}\"}}";
            var (response, rs) = await NZHttpHelper.BasePostRequest<BarginBookingInfoRoot>(postData, _setting, ApiEnum.Detail);
            if (response != null && response.status != "OK" && !string.IsNullOrEmpty(response.error))
            {
                Log.Error(response.error);
            }
            return (response, rs);
        }

        /// <summary>
        /// 构建门店
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BuildAllLocation()
        {
            var postModel = new
            {
                method = "step1"
            };
            var (rs, str) = await NZHttpHelper.BasePostRequest<LocationRS>(JsonConvert.SerializeObject(postModel), _setting, ApiEnum.Location);
            if (rs != null && rs.Results != null)
            {
                List<NZLocation> changeList = new List<NZLocation>();
                List<CarLocationSupplier> carLoc = new List<CarLocationSupplier>();
                var rsLoc = rs.Results.Locations;
                // var locFilter = string.Join(",", rsLoc.Select(n => $"'{n.Location}'"));
                var supplier_re = _cr_repository.GetRepository<CarLocationSupplier>();

                var spLoc = await supplier_re.Query().Where(n => n.Supplier == suppiler).ToListAsync();
                var allCity = await _cr_repository.GetRepository<CarCity>().Query().ToListAsync();
                var dbLoc = await _repository.GetRepository<NZLocation>().Query().ToListAsync();
                foreach (var r in rsLoc)
                {
                    try
                    {
                        var city = allCity.FirstOrDefault(n => n.CityNameEn.ToLower() == r.Location.ToLower());
                        var exist = dbLoc.FirstOrDefault(n => n.SuppLocId == r.Id.ToString());
                        if (exist == null)
                        {
                            exist = new NZLocation();
                            exist.CreateTime = DateTime.Now;
                        }
                        locationMap.TryGetValue(r.Id, out NZLocation exdb);
                        exist.LocationName = r.Location;
                        if (exdb != null)
                        {
                            exist.CountryCode = exdb.CountryCode;
                            exist.CountryName = exdb.CountryName;
                            exist.CityName = exdb.CityName;
                            exist.CityID = exdb.CityID;
                            exist.Latitude = exdb.Latitude;
                            exist.Longitude = exdb.Longitude;
                            exist.Address = exdb.Address;
                            exist.Airport = exdb.Airport;
                            exist.AirportCode = exdb?.AirportCode;
                        }
                        else
                        {
                            Log.Error($"没有找到对应的配置经纬度项目");
                        }
                        exist.Telephone = r.Phone;
                        exist.OpenTime = r.OfficeOpeningTime;
                        exist.CloseTime = r.OfficeClosingTime;
                        exist.SuppLocId = r.Id.ToString();
                        exist.VendorLocId = r.Id.ToString();
                        exist.Vendor = suppiler;
                        exist.Email = r.Email;
                        exist.UpdateTime = DateTime.Now;
                        exist.OperationTime = BuildOperationTime(rs.Results.OfficeTimes, r.Id);
                        changeList.Add(exist);

                        //供应商标准表
                        var spModel = spLoc.FirstOrDefault(n => n.SuppLocId == r.Id.ToString());
                        var isadd = false;
                        if (spModel == null)
                        {
                            spModel = new CarLocationSupplier();
                            isadd = true;
                            spModel.Status = 0;
                        }
                        spModel.LocationName = exist.LocationName;
                        spModel.CountryCode = exist.CountryCode;
                        if (string.IsNullOrWhiteSpace(spModel.CountryCode))
                        {
                            spModel.CountryCode = "US";
                        }
                        spModel.CountryName = exist.CountryName;
                        spModel.CityName = exist.CityName;
                        spModel.Address = exist.Address;
                        spModel.Latitude = exist.Latitude?.ToString();
                        spModel.Longitude = exist.Longitude?.ToString();
                        spModel.Airport = string.IsNullOrWhiteSpace(exist.AirportCode) ? false : true;
                        spModel.AirportCode = exist.AirportCode;
                        spModel.StateCode = exist.VendorLocId;
                        spModel.PostalCode = exist.PostalCode;
                        spModel.Telephone = exist.Telephone;
                        spModel.CityId = city == null ? 0 : city.CityId;
                        spModel.OperationTime = exist.OperationTime;
                        spModel.Supplier = suppiler;
                        spModel.VendorLocId = exist.VendorLocId;
                        spModel.SuppLocId = exist.SuppLocId;
                        spModel.UpdateTime = DateTime.Now;
                        spModel.OperationTime = exist.OperationTime;

                        if (isadd)
                        {
                            spModel.CreateTime = DateTime.Now;
                        }
                        carLoc.Add(spModel);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"门店更新错误{ex.Message}");
                    }
                }
                await _repository.GetRepository<NZLocation>().BatchUpdateAsync(changeList.Where(n => n.LocationId > 0).ToList());

                await _repository.GetRepository<NZLocation>().BatchInsertAsync(changeList.Where(n => n.LocationId == 0 || n.LocationId == null).ToList());
                await _cr_repository.GetRepository<CarLocationSupplier>().BatchUpdateAsync(carLoc.Where(n => n.LocationId > 0).ToList());

                await _cr_repository.GetRepository<CarLocationSupplier>().BatchInsertAsync(carLoc.Where(n => n.LocationId == 0 || n.LocationId == null).ToList());

                //车类型管理
                var allcategory = await _repository.GetRepository<NZCategoryTypes>().Query().ToListAsync();
                foreach (var cate in rs.Results.CategoryTypes)
                {
                    var existcate = allcategory.FirstOrDefault(n => n.CategoryTypesId == cate.Id);
                    if (existcate == null)
                    {
                        existcate = new NZCategoryTypes()
                        {
                            CategoryTypesId = cate.Id,
                            DisplayOrder = cate.DisplayOrder,
                            VehicleCategoryType = cate.VehicleCategoryType
                        };
                        await _repository.GetRepository<NZCategoryTypes>().InsertAsync(existcate);
                    }
                    else
                    {
                        existcate.CategoryTypesId = cate.Id;
                        existcate.DisplayOrder = cate.DisplayOrder;
                        existcate.VehicleCategoryType = cate.VehicleCategoryType;
                        await _repository.GetRepository<NZCategoryTypes>().UpdateAsync(existcate);
                    }
                }
                //驾驶年龄插入
                var allageSetting = await _repository.GetRepository<NZDriverAge>().Query().ToListAsync();
                foreach (var age in rs.Results.DriverAges)
                {
                    var existAge = allageSetting.FirstOrDefault(n => n.AgeId == age.Id);
                    if (existAge == null)
                    {
                        existAge = new NZDriverAge()
                        {
                            AgeId = age.Id,
                            DriverAge = age.driverage,
                            IsDefault = age.isdefault.ToString()
                        };
                        await _repository.GetRepository<NZDriverAge>().InsertAsync(existAge);
                    }
                    else
                    {
                        existAge.AgeId = age.Id;
                        existAge.DriverAge = age.driverage;
                        existAge.IsDefault = age.isdefault.ToString();
                        await _repository.GetRepository<NZDriverAge>().UpdateAsync(existAge);
                    }
                }
            }
            return true;
        }

        public Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public async Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            var createOrderRes = new StdCreateOrderRS();
            try
            {
                createOrderRQ.PickUpDateTime = Convert.ToDateTime(createOrderRQ.PickUpDateTime);
                createOrderRQ.ReturnDateTime = Convert.ToDateTime(createOrderRQ.ReturnDateTime);
                var rateCode = Encryptor.Decrypt(createOrderRQ.RateCode);
                var extensionsInfo = rateCode.Split('|');
                var data = await VehRes(createOrderRQ, extensionsInfo);

                if (data != null && data.results != null && !data.results.reservationref.IsNullOrEmpty())
                {
                    decimal price = 0;
                    string suppCurrency = "";
                    BarginBookinginfoItem bookinginfo = new BarginBookinginfoItem();
                    try
                    {
                        var (queryOrderRes, source) = await VehRetRes(data.results.reservationref);
                        bookinginfo = queryOrderRes?.results?.bookinginfo?.FirstOrDefault();
                        price = bookinginfo.totalcost;
                        suppCurrency = bookinginfo.currencyname;
                        //if (bookinginfo == null)
                        //    return createOrderRes;
                    }
                    catch (Exception ex)
                    {
                    }
                    var suppOrderStatus = bookinginfo?.reservationstatus;//供应商订单状态
                    createOrderRes = new StdCreateOrderRS()
                    {
                        OrderSuc = true,
                        SuppOrderId = data.results.reservationref,
                        SuppOrderStatus = suppOrderStatus,
                        SuppConfirmed = suppOrderStatus == "Reservation",
                        SuppTotalPrice = price,
                        SuppCurrency = suppCurrency
                    };
                    BarginCarOrder order = new BarginCarOrder
                    {
                        Reservationref = createOrderRes.SuppOrderId,
                        SuppOrderStatus = createOrderRes.SuppOrderStatus,
                        Reservationdocumentno = bookinginfo?.reservationdocumentno,
                        Reservationno = bookinginfo?.reservationno,
                        PickUpDateTime = createOrderRQ.PickUpDateTime,
                        ReturnDateTime = createOrderRQ.ReturnDateTime,
                        CountryCode = createOrderRQ.CitizenCountryCode,
                        ReturnLocation = createOrderRQ.ReturnLocation.SuppLocId,
                        PickUpLocation = createOrderRQ.PickUpLocation.SuppLocId,
                        LastName = createOrderRQ.LastName,
                        FirstName = createOrderRQ.FirstName,
                        Vendor = _vendor.ToString(),
                        DriverAge = createOrderRQ.DriverAge,
                        VehiclecategoryId = Convert.ToInt32(extensionsInfo[3]),
                        Email = createOrderRQ.Email,
                        ExtrakmsId = Convert.ToInt32(extensionsInfo[5]),
                        InsuranceId = Convert.ToInt32(extensionsInfo[4]),
                        Isonrequest = bookinginfo?.isonrequest,
                        VehiclecategorytypeId = Convert.ToInt32(extensionsInfo[0]),
                    };
                    await _repository.GetRepository<BarginCarOrder>().InsertAsync(order);
                }
                else
                {
                    createOrderRes.OrderSuc = false;
                    createOrderRes.Message = "创建订单异常-" + data?.error;
                }
                return createOrderRes;
            }
            catch (Exception ex)
            {
                Log.Error($"CreateOrder出错-{JsonConvert.SerializeObject(createOrderRQ)}", ex);
                createOrderRes.OrderSuc = false;
                createOrderRes.Message = "创建订单异常-" + ex.ToString();
                return createOrderRes;
            }
        }

        public async Task<List<CarLocationSupplier>> GetAllLocation()
        {
            var spLoc = await _cr_repository.GetRepository<CarLocationSupplier>().Query().Where(n => n.Supplier == suppiler).ToListAsync();
            return spLoc;
        }

        private async Task<int> GetSupplierAgeId(int age)
        {
            var descAge = 4;
            var drivetype = await _repository.GetRepository<NZDriverAge>().Query().ToListAsync();
            var dbAge = drivetype.FirstOrDefault(n => n.DriverAge == age);
            if (dbAge != null)
            {
                descAge = Convert.ToInt32(dbAge.AgeId.Value);
            }
            return descAge;
        }

        private Dictionary<string, EnumCarEquipType> dict = new Dictionary<string, EnumCarEquipType>()
        {
              {"1995",EnumCarEquipType.CSI },
              {"1816",EnumCarEquipType.ChildSeat },
              {"1817",EnumCarEquipType.BoosterSeat }
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
        }

        /// <summary>
        /// 查询车辆集合。
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<BarginVehicleSeachResponse> VehAvailRate(BarginQueryDto dto)
        {
            var availRateRQ = dto.SourceQuery;

            var postData = JsonConvert.SerializeObject(availRateRQ);
            var (result, response) = await NZHttpHelper.BasePostRequest<BarginVehicleSeachResponse>(postData, _setting, Share.Dtos.ApiEnum.List);
            if (result != null && result?.status != "OK" && !string.IsNullOrEmpty(result?.error))
            {
                Log.Error(result.error);
            }
            return result;
        }

        /// <summary>
        /// 获取具体车辆的报价规则
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<BarginVehicleSeachResponse> VehRateRule(VehicleSeachRequest availRateRQ)
        {
            availRateRQ.method = Method.step3.ToString();
            var postData = Newtonsoft.Json.JsonConvert.SerializeObject(availRateRQ);
            var (result, response) = await NZHttpHelper.BasePostRequest<BarginVehicleSeachResponse>(postData, _setting, ApiEnum.Rule);
            if (result != null && result.status != "OK" && !string.IsNullOrEmpty(result.error))
            {
                throw new Exception(result.error);
            }
            return result;
        }

        public async Task<List<StdVehicle>> GetVehiclesByloc(BarginQueryDto querydto)
        {
            List<StdVehicle> sVehs = new List<StdVehicle>();
            try
            {
                var availRateRes = await VehAvailRate(querydto);
                if (availRateRes.results != null)
                {
                    var result = availRateRes.results;
                    int insuranceoptionsid = 0, kmchargesid = 0;
                    var vendorAvails = result.availablecars;
                    string _currency = result.locationfees?.FirstOrDefault()?.currencyname ?? "AUD";
                    BarginVehicleSeachResponse rules = new BarginVehicleSeachResponse();
                    if (vendorAvails == null)
                    {
                        return sVehs;
                    }
                    foreach (var vehicle in vendorAvails)
                    {
                        if (vehicle.available == 0)
                        {
                            continue;
                        }

                        var priceType = EnumCarPriceType.NetRate;
                        var sippcode = vehicle.sippcode;
                        var sVeh = new StdVehicle()
                        {
                            Supplier = (EnumCarSupplier)suppiler,
                            RateCode = Guid.NewGuid().ToString(),
                            VehicleCode = vehicle.sippcode,
                            PictureURL = "https:" + vehicle.imageurl,
                            MinDriverAge = vehicle.minimumage,
                            MaxDriverAge = vehicle.maximumage,
                            BaggageQuantity = vehicle.numberoflargecases,
                            SmallBaggageQuantity = vehicle.numberofsmallcases,
                            PassengerQuantity = vehicle.numberofadults,
                            FuelPolicy = EnumCarFuelPolicy.FullToFull,
                            VendorLogo = "https://ctimg-svg.cartrawler.com/supplier-images/bargain_car_rentals.svg",

                            TotalCharge = new StdTotalCharge()
                            {
                                PayType = EnumCarPayType.PayOnArrival,
                                PriceType = priceType,
                                Currency = _currency,
                                TotalAmount = vehicle.totalrateafterdiscount,
                            }.AddOtherAmount(EnumCarCoverageType.Deposit, EnumCarPayWhen.NoNeed, _currency, Convert.ToDecimal("500")),

                            ProductCode = $"{suppiler + querydto.startLoc.VendorLocId + vehicle.sippcode}",

                            FuelType = (EnumCarFuelType)SIPPHelper.SIPPCodeAnalysis(sippcode, 0),//"需要把车辆整到枚举才可知道SIPP,车辆表对应不上"   用SIPP第四位数对应好像也可以

                            TransmissionType = (EnumCarTransmissionType)SIPPHelper.SIPPCodeAnalysis(sippcode, 5),//需对应车辆表格  用SIPP第三位数对应好像也可以
                                                                                                                 //AirConditioning = true,
                            DriveType = EnumCarDriveType.None,
                            VehicleCategory = (EnumCarVehicleCategory)SIPPHelper.SIPPCodeAnalysis(sippcode, 6),//需对应车辆表格 用SIPP第二位数对应好像也可以
                            VehicleClass = (EnumCarVehicleClass)SIPPHelper.SIPPCodeAnalysis(sippcode, 4),//需对应车辆表格  用SIPP第一位数对应好像也可以
                                                                                                         //VehicleGroup = SippHelper.GetVehicleGroup(sippcode),//需对应车辆表格  用SIPP第一位数对应好像也可以
                            VehicleName = vehicle.categoryfriendlydescription,

                            //RateCode = vehiclecategoryid + "|" + totalrateafterdiscount, //车辆id|折后总价 需要校验是否变价

                            Vendor = _vendor,
                            VendorName = _vendor.ToString(),
                            DoorCount = (EnumCarDoorCount)SIPPHelper.SIPPCodeAnalysis(sippcode, 2)//需对应车辆表格  用SIPP第二位数对应好像也可以
                        };

                        sVeh.CancelPolicy = new StdCancelPolicy
                        {
                            CancelType = EnumCarCancelType.FreeCancel,
                            PickUpBeforeHours = 48,
                            Rules = new List<StdCancelRule> { new StdCancelRule() { DeductType = EnumCarDeductType.Free, StartTime = DateTime.Now, EndTime = querydto.vehicleRQ.PickUpDateTime.AddHours(-48) } }
                        };

                        #region Location

                        var pickUp = querydto.startLoc;
                        var endLoc = querydto.endLoc;
                        sVeh.Location = new StdLocation()
                        {
                            PickUp = new StdLocationInfo()
                            {
                                SuppLocId = pickUp.SuppLocId,
                                VendorLocId = pickUp.VendorLocId,
                                LocationName = pickUp.LocationName,
                                Latitude = pickUp.Latitude,
                                Longitude = pickUp.Longitude,
                                LocationId = pickUp.LocationId.Value,
                                CityId = pickUp.CityId.HasValue ? pickUp.CityId.Value : 0,
                                PostalCode = pickUp.PostalCode,
                                StateCode = pickUp.StateCode,
                                CityName = pickUp.CityName,
                                CountryCode = pickUp.CountryCode,
                                CloseTime = pickUp.CloseTime,
                                OpenTime = pickUp.OpenTime,
                                CountryName = pickUp.CountryName,
                                //OperationTimes = JsonConvert.DeserializeObject<List<StdOperationTime>>(pickUp.OperationTime)
                            },
                            DropOff = new StdLocationInfo()
                            {
                                SuppLocId = endLoc.SuppLocId,
                                VendorLocId = endLoc.VendorLocId,
                                LocationName = endLoc.LocationName,
                                Latitude = endLoc.Latitude,
                                Longitude = endLoc.Longitude,
                                LocationId = endLoc.LocationId.Value,
                                CityId = endLoc.CityId.HasValue ? endLoc.CityId.Value : 0,
                                PostalCode = endLoc.PostalCode,
                                StateCode = endLoc.StateCode,
                                CityName = endLoc.CityName,
                                CountryCode = endLoc.CountryCode,
                                CloseTime = endLoc.CloseTime,
                                OpenTime = endLoc.OpenTime,
                                CountryName = endLoc.CountryName,
                                //OperationTimes = JsonConvert.DeserializeObject<List<StdOperationTime>>(endLoc.OperationTime)
                            }
                        };

                        #endregion Location

                        try
                        {
                            var availRateRQ = querydto.SourceQuery;
                            availRateRQ.vehiclecategoryid = vehicle.vehiclecategoryid;
                            rules = await VehRateRule(availRateRQ);
                            if (rules != null && rules.results != null)
                            {
                                var rule = rules.results;

                                #region PricedCoverages

                                var pricedCoverages = rule.insuranceoptions;
                                if (pricedCoverages != null && pricedCoverages.Count > 0)
                                {
                                    foreach (var charge in pricedCoverages)
                                    {
                                        insuranceoptionsid = charge.isdefault ? charge.id : 0;
                                        var coverageType = EnumCarCoverageType.None;
                                        var coverageCode = charge.name;

                                        sVeh.PricedCoverages.Add(new StdPricedCoverage()
                                        {
                                            CoverageType = coverageType,
                                            CoverageDescription = charge.feedescription,
                                            Description = charge.feedescription,
                                            CurrencyCode = _currency,
                                            Amount = charge.totalinsuranceamount,
                                            TaxInclusive = charge.gst,
                                            IncludedInRate = false,
                                        });
                                    }
                                }

                                #endregion PricedCoverages

                                #region VehicleCharges

                                var vehicleCharges = rule.mandatoryfees;
                                if (vehicleCharges != null)
                                {
                                    foreach (var vc in vehicleCharges)
                                    {
                                        var purpose = EnumCarCoverageType.None;

                                        if (vc.name == "Gas Return Charge")//原始供应商租车币种价格
                                        {
                                            //usertodo
                                            //purpose = (EnumCarCoverageType)EnumCarCoverageTypeExt.GRC;
                                            sVeh.TotalCharge.AddOtherAmount(purpose, EnumCarPayWhen.NoNeed, _currency, vc.totalfeeamount.Value);
                                        }
                                        else
                                        {
                                        }
                                        var stdVehicleCharge = new StdVehicleCharge()
                                        {
                                            Purpose = purpose,
                                            PurposeDescription = vc.feedescription,
                                            Description = vc.name,
                                            CurrencyCode = _currency,
                                            Amount = vc.totalfeeamount == null ? 0 : vc.totalfeeamount.Value,
                                            TaxInclusive = vc.gst,
                                            IncludedInEstTotalInd = false
                                        };
                                        sVeh.VehicleCharges.Add(stdVehicleCharge);
                                    }
                                }

                                #endregion VehicleCharges

                                #region PricedEquips

                                var equips = rule.optionalfees;
                                if (equips != null)
                                {
                                    foreach (var equip in equips)
                                    {
                                        var equipType = BuildPricedEquip(equip.id.ToString());
                                        if (equipType == EnumCarEquipType.None)
                                        {
                                            Log.Error($"存在没有处理的设备{JsonConvert.SerializeObject(equip)}");
                                            continue;
                                        }
                                        //usertodo
                                        var eq = new StdPricedEquip()
                                        {
                                            EquipType = equipType,
                                            EquipDescription = equip.name,
                                            Currency = _currency,
                                            UnitPrice = equip.totalfeeamount,
                                            IncludedInEstTotalInd = false,//不包含在总价中，表示可选
                                            Unit = EnumCarPeriodUnitName.Day,
                                            MaxPrice = equip.maximumprice,
                                            MaxQuantity = 1
                                        };
                                        sVeh.PricedEquips.Add(eq);
                                    }
                                }

                                #endregion PricedEquips

                                #region kmcharges

                                if (rule.kmcharges != null && rule.kmcharges.Count > 0)
                                {
                                    var limited = rule.kmcharges.Where(o => o.isdefault).FirstOrDefault();
                                    kmchargesid = limited.id;
                                    sVeh.RateDistance = new StdRateDistance
                                    {
                                        Unlimited = limited.description == "Unlimited kilometres" || limited.description == "Unlimited Kms" ? true : false,
                                        DistUnitName = limited.mileagedesc == "Kms" ? EnumCarDistUnitName.Km : EnumCarDistUnitName.Mile,
                                        VehiclePeriodUnitName = EnumCarPeriodUnitName.Day,
                                        Amount = limited.feeforeachadditionalkm,
                                        Description = limited.description
                                    };
                                }

                                #endregion kmcharges
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        #region vehicle

                        // 车型组
                        sVeh.VehicleGroup = CarCommonHelper.GetVehicleGroup(vehicle.sippcode);
                        sVeh.Vendor = EnumCarVendor.Bargain_Car_Rentals; //usertodo
                        sVeh.VendorName = EnumCarVendor.Bargain_Car_Rentals.ToString(); //usertodo

                        #endregion vehicle

                        sVeh.RateCode = Encryptor.Encrypt($"{vehicle.vehiclecategorytypeid}|{pickUp.VendorLocId}|{endLoc.VendorLocId}|{vehicle.vehiclecategoryid}|{insuranceoptionsid}|{kmchargesid}");
                        sVeh.TermsAndConditionsRef = Encryptor.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(querydto.SourceQuery));
                        sVehs.Add(sVeh);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return sVehs;
        }

        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            Log.Information($"NZ 接收到参数{JsonConvert.SerializeObject(vehicleRQ)}");
            var locList = await GetAllLocation();
            var (startLocList, endLocList) = await CommonLocationHelper.GetLocaiton(vehicleRQ, locList);
            var ageid = await GetSupplierAgeId(vehicleRQ.DriverAge);
            int batchSize = 1; // 限制 10个任务跑
            List<Task> runTasks = new List<Task>();
            List<StdVehicle> res = new List<StdVehicle>();
            List<StdVehicle> sVehs = new List<StdVehicle>();
            var allCatecory = await _repository.GetRepository<NZCategoryTypes>().Query().ToListAsync();
            foreach (var cate in allCatecory)
            {
                foreach (var start in startLocList)
                {
                    if (vehicleRQ.PickUpLocationCode == vehicleRQ.ReturnLocationCode)
                    {
                        var querydto = new BarginQueryDto
                        {
                            SourceQuery = new VehicleSeachRequest()
                            {
                                dropoffdate = vehicleRQ.ReturnDateTime.ToString("dd/MM/yyyy"),
                                dropofftime = vehicleRQ.ReturnDateTime.ToString("HH:mm"),
                                pickupdate = vehicleRQ.PickUpDateTime.ToString("dd/MM/yyyy"),
                                pickuptime = vehicleRQ.PickUpDateTime.ToString("HH:mm"),
                                pickuplocationid = start.VendorLocId,
                                dropofflocationid = start.VendorLocId,
                                method = Method.step2.ToString(),
                                ageid = ageid,
                                vehiclecategorytypeid = cate.CategoryTypesId.ToString(),
                            },
                            startLoc = start,
                            endLoc = start,
                            vehicleRQ = vehicleRQ
                        };
                        var isCheck = !vehicleRQ.ReferenceId.IsNullOrEmpty();//是否验单
                                                                             //直接访问接口，接口已有缓存
                        var task = Task.Run(async () =>
                        {
                            var availRateRes = await GetVehiclesByloc(querydto);
                            sVehs.AddRange(availRateRes);
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
                            var querydto = new BarginQueryDto
                            {
                                SourceQuery = new VehicleSeachRequest()
                                {
                                    dropoffdate = vehicleRQ.ReturnDateTime.ToString("dd/MM/yyyy"),
                                    dropofftime = vehicleRQ.ReturnDateTime.ToString("HH:mm"),
                                    pickupdate = vehicleRQ.PickUpDateTime.ToString("dd/MM/yyyy"),
                                    pickuptime = vehicleRQ.PickUpDateTime.ToString("HH:mm"),
                                    pickuplocationid = start.VendorLocId,
                                    dropofflocationid = start.VendorLocId,
                                    method = Method.step2.ToString(),
                                    ageid = ageid,
                                    vehiclecategorytypeid = cate.CategoryTypesId.ToString()
                                },
                                startLoc = start,
                                endLoc = end,
                                vehicleRQ = vehicleRQ
                            };
                            var task = Task.Run(async () =>
                            {
                                var availRateRes = await GetVehiclesByloc(querydto);
                                sVehs.AddRange(availRateRes);
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
            }
            await Task.WhenAll(runTasks);
            Log.Information($"返回{sVehs.Count}条数据");
            return sVehs;
        }

        public Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public Task Test()
        {
            throw new NotImplementedException();
        }
    }
}