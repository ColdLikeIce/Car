using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.ACE;
using Microsoft.AspNetCore.Mvc;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ACECarController : ControllerBase
    {
        private readonly IACEApi _carSupplierApi;

        public ACECarController(IACEApi carSupplierApi)
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

        [HttpGet("BuildLocation")]
        public async Task BuildLocation()
        {
            await _carSupplierApi.BuildAllLocation();
        }
    }
}