using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.BarginCar;
using HeyTripCarWeb.Supplier.NZ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NZController : ControllerBase
    {
        private readonly INZApi _carSupplierApi;

        private readonly JwtHelper _jwtHelper;
        private readonly JwtConfig _config;

        public NZController(INZApi carSupplierApi, IOptions<JwtConfig> options, JwtHelper jwtHelper)
        {
            _carSupplierApi = carSupplierApi;
            _jwtHelper = jwtHelper;
            _config = options.Value;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("GetVehicles")]
        public async Task<List<StdVehicle>> GetVehiclesAsync([FromBody] StdGetVehiclesRQ dto)
        {
            return await _carSupplierApi.GetVehiclesAsync(dto);
        }

        [HttpPost("CreateOrder")]
        public async Task<StdCreateOrderRS> CreateOrderAsync([FromBody] StdCreateOrderRQ dto)
        {
            return await _carSupplierApi.CreateOrderAsync(dto);
        }

        [HttpPost("CancelOrder")]
        public async Task<StdCancelOrderRS> CancelOrderAsync([FromBody] StdCancelOrderRQ dto)
        {
            return await _carSupplierApi.CancelOrderAsync(dto);
        }

        [HttpPost("QueryOrder")]
        public async Task<StdQueryOrderRS> QueryOrderAsync([FromBody] StdQueryOrderRQ dto)
        {
            return await _carSupplierApi.QueryOrderAsync(dto);
        }

        [HttpGet("InitLocation")]
        public async Task InitLocation()
        {
            await _carSupplierApi.BuildAllLocation();
        }

        [HttpGet("test")]
        public async Task test()
        {
            await _carSupplierApi.Test();
        }
    }
}