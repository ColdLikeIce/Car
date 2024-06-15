using HeyTripCarWeb.Share;
using Microsoft.AspNetCore.Mvc;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ABGCarController : ControllerBase
    {
        private readonly ICarSupplierApi _carSupplierApi;

        public ABGCarController(ICarSupplierApi carSupplierApi)
        {
            _carSupplierApi = carSupplierApi;
        }

        [HttpPost("GetVehicles")]
        public async Task<string> GetVehiclesAsync([FromBody] StdGetVehiclesRQ dto)
        {
            await _carSupplierApi.GetVehiclesAsync(dto);
            return "";
        }
    }
}