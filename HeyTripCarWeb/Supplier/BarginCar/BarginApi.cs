using CommonCore.Mapper;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;
using HeyTripCarWeb.Supplier.ACE.Models.RSs;
using HeyTripCarWeb.Supplier.BarginCar.Config;
using HeyTripCarWeb.Supplier.BarginCar.Model.Dbs;
using HeyTripCarWeb.Supplier.BarginCar.Model.RSs;
using HeyTripCarWeb.Supplier.BarginCar.Util;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HeyTripCarWeb.Supplier.BarginCar
{
    public class BarginApi : IBarginApi
    {
        private readonly BarginCarAppSetting _setting;
        private readonly IMapper _mapper;
        private readonly IRepository<CarLocationSupplier> _supplierCatRe;
        private readonly IRepository<CarCity> _CatCityRe;

        private readonly IRepository<BargainLocation> _locRepository;

        public BarginApi(IOptions<BarginCarAppSetting> options, IMapper mapper, IRepository<CarLocationSupplier> supplierCatRe,
           IRepository<CarCity> catCityRe, IRepository<BargainLocation> locRepository)
        {
            _setting = options.Value;
            _mapper = mapper;
            _supplierCatRe = supplierCatRe;
            _CatCityRe = catCityRe;
            _locRepository = locRepository;
        }

        public async Task<bool> BuildAllLocation()
        {
            var postModel = new
            {
                method = "step1"
            };
            var rs = await BarginHttpHelper.BasePostRequest<LocationRS>(JsonConvert.SerializeObject(postModel), _setting);
            if (rs != null && rs.Results != null)
            {
                var rsLoc = rs.Results.Locations;
                var locFilter = string.Join(",", rsLoc.Select(n => $"'{n.Location}'"));
                var dbs = await _supplierCatRe.GetListBySqlAsync($"select * from CarRental.dbo.Car_Location_Suppliers where locationname in ({locFilter})", null);
                var spLoc = await _supplierCatRe.GetListBySqlAsync("select * from CarRental.dbo.Car_Location_Suppliers where supplier =@supplier", new { supplier = (int)EnumCarSupplier.BarginCar });
                var allCity = await _CatCityRe.GetAllAsync();
                var dbLoc = await _locRepository.GetAllAsync();
                foreach (var r in rsLoc)
                {
                    var city = allCity.FirstOrDefault(n => n.CityNameEn.ToLower() == r.Location.ToLower());
                    var exist = dbLoc.FirstOrDefault(n => n.SuppLocId == r.Id.ToString());
                    BargainLocation location = new BargainLocation();
                    if (exist != null)
                    {
                        location = exist;
                    }
                    location.LocationName = r.Location;
                    var exdb = dbs.FirstOrDefault(n => n.LocationName == r.Location && !string.IsNullOrWhiteSpace(n.Longitude));
                    if (exdb != null)
                    {
                        location.CountryCode = exdb.CountryCode;
                        location.CountryName = exdb.CountryName;
                        location.CityName = exdb.CityName;
                        location.CityID = exdb.CityId;
                        location.Latitude = exdb.Latitude;
                        location.Longitude = exdb.Longitude;
                        location.Address = exdb.Address;

                        if (r.Location.Contains("Airport"))
                        {
                            location.Airport = true;
                            location.AirportCode = exdb.AirportCode;
                        }
                    }
                    location.Telephone = r.Phone;
                    location.OpenTime = r.OfficeOpeningTime;
                    location.CloseTime = r.OfficeClosingTime;
                    location.SuppLocId = r.Id.ToString();
                    location.VendorLocId = r.Id.ToString();
                    location.Vendor = (int)(int)EnumCarSupplier.BarginCar;
                    location.Email = r.Email;
                    location.UpdateTime = DateTime.Now;
                    if (exist != null)
                    {
                        await _locRepository.UpdateAsync(location);
                    }
                    else
                    {
                        location.CreateTime = DateTime.Now;
                        await _locRepository.InsertAsync(location);
                    }

                    //供应商标准表
                    var spModel = spLoc.FirstOrDefault(n => n.VendorLocId == r.Id.ToString());
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
                    spModel.VendorLocId = location.LocationId.ToString();
                    spModel.SuppLocId = location.SuppLocId;
                    spModel.Supplier = ((int)EnumCarSupplier.BarginCar).ToString();
                    spModel.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    if (isadd)
                    {
                        spModel.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        await _supplierCatRe.InsertAsync(spModel);
                    }
                    else
                    {
                        await _supplierCatRe.UpdateAsync(spModel);
                    }
                }
            }
            return true;
        }

        public async Task<List<CarLocationSupplier>> GetAllLocation()
        {
            var spLoc = await _supplierCatRe.GetListBySqlAsync("select * from CarRental.dbo.Car_Location_Suppliers where supplier =@supplier", new { supplier = (int)EnumCarSupplier.BarginCar });
            return spLoc;
        }

        public Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            var locList = await GetAllLocation();
            var (startLocList, endLocList) = await CommonLocationHelper.GetLocaiton(vehicleRQ, locList);
            foreach (var start in startLocList)
            {
                /*     if (vehicleRQ.PickUpLocationCode == vehicleRQ.ReturnLocationCode)
                     {
                         var availRateRQ = new VehicleSeachRequest()
                         {
                             dropOffDateTime = vehicleRQ.ReturnDateTime.ToString("yyyy-MM-ddTHH") + "%3A00%3A00%2B00%3A00",
                             pickUpDateTime = vehicleRQ.PickUpDateTime.ToString("yyyy-MM-ddTHH") + "%3A00%3A00%2B00%3A00",
                             pickUpLocation = start.StateCode,
                             dropOffLocation = start.StateCode,
                             driverAge = vehicleRQ.DriverAge,
                         };
                         var isCheck = !vehicleRQ.ReferenceId.IsNullOrEmpty();//是否验单
                                                                              //直接访问接口，接口已有缓存
                         var task = Task.Run(async () =>
                         {
                             var availRateRes = await GetVehiclesByloc(vehicleRQ, availRateRQ, start, start, allproducts);
                             sVehs.AddRange(availRateRes);
                         });
                         runTasks.Add(task);
                         if (runTasks.Count > batchSize)
                         {
                             await Task.WhenAll(runTasks);
                             runTasks = new List<Task>();
                         }
                     }
                     else
                     {
                         foreach (var end in endLocList)
                         {
                             var availRateRQ = new VehicleSeachRequest()
                             {
                                 dropOffDateTime = vehicleRQ.ReturnDateTime.ToString("yyyy-MM-ddTHH") + "%3A00%3A00%2B00%3A00",
                                 pickUpDateTime = vehicleRQ.PickUpDateTime.ToString("yyyy-MM-ddTHH") + "%3A00%3A00%2B00%3A00",
                                 pickUpLocation = start.StateCode,
                                 dropOffLocation = end.StateCode,
                                 driverAge = vehicleRQ.DriverAge,
                                 typeId = categoryType.ToString()
                             };
                             var task = Task.Run(async () =>
                             {
                                 var availRateRes = await GetVehiclesByloc(vehicleRQ, availRateRQ, start, end, allproducts);
                                 sVehs.AddRange(availRateRes);
                             });
                             runTasks.Add(task);
                             if (runTasks.Count > batchSize)
                             {
                                 await Task.WhenAll(runTasks);
                                 runTasks = new List<Task>();
                             }
                         }
                     }*/
            }
            throw new NotImplementedException();
        }

        public Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }
    }
}