using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.ABG;
using Microsoft.AspNetCore.Mvc;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ABGCarController : ControllerBase
    {
        private readonly IABGApi _carSupplierApi;

        public ABGCarController(IABGApi carSupplierApi)
        {
            _carSupplierApi = carSupplierApi;
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
        public async Task<bool> InitLocation()
        {
            return await _carSupplierApi.InitLocation();
        }

        [HttpGet("InitLocationOperationTimes")]
        public async Task<bool> InitLocationOperationTimes()
        {
            return await _carSupplierApi.InitLocationOperationTimes();
        }

        [HttpGet("InitCreditCardPolicy")]
        public async Task InitCreditCardPolicy()
        {
            await _carSupplierApi.InitCreditCardPolicy();
        }

        [HttpGet("InitYoungDriver")]
        public async Task InitYoungDriver()
        {
            await _carSupplierApi.InitYoungDriver();
        }

        public async Task DownLoadFtpFile()
        {
        }
    }
}