using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.BarginCar;
using HeyTripCarWeb.Supplier.Sixt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Controllers
{
    [ApiController]
    [Route("/api/Sixt")]
    [Authorize]
    public class SixtController : ControllerBase
    {
        private readonly ISixtApi _carSupplierApi;

        private readonly JwtHelper _jwtHelper;
        private readonly JwtConfig _config;

        public SixtController(ISixtApi carSupplierApi, IOptions<JwtConfig> options, JwtHelper jwtHelper)
        {
            _carSupplierApi = carSupplierApi;
            _jwtHelper = jwtHelper;
            _config = options.Value;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("GetVehicles")]
        [Authorize]
        public async Task<List<StdVehicle>> GetVehiclesAsync([FromBody] StdGetVehiclesRQ dto)
        {
            return await _carSupplierApi.GetVehiclesAsync(dto);
        }

        [HttpPost("CreateOrder")]
        [Authorize]
        public async Task<StdCreateOrderRS> CreateOrderAsync([FromBody] StdCreateOrderRQ dto)
        {
            return await _carSupplierApi.CreateOrderAsync(dto);
        }

        [HttpPost("CancelOrder")]
        [Authorize]
        public async Task<StdCancelOrderRS> CancelOrderAsync([FromBody] StdCancelOrderRQ dto)
        {
            return await _carSupplierApi.CancelOrderAsync(dto);
        }

        [HttpPost("QueryOrder")]
        [Authorize]
        public async Task<StdQueryOrderRS> QueryOrderAsync([FromBody] StdQueryOrderRQ dto)
        {
            return await _carSupplierApi.QueryOrderAsync(dto);
        }

        [HttpGet("Init")]
        public async Task InitLocation()
        {
            await _carSupplierApi.BuildAllLocation();
        }

        [HttpGet("test")]
        public async Task test()
        {
            await _carSupplierApi.test();
        }
    }
}