using CarrentalWeb.Impl;
using Microsoft.AspNetCore.Mvc;
using XiWan.Car.BusinessShared.Stds;

namespace CarrentalWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarrentalController : ControllerBase
    {
        private readonly ICarSupplierApi _carDomain;

        public CarrentalController(ICarSupplierApi carDomain)
        {
            _carDomain = carDomain;
        }

        [HttpGet(Name = "GetVehicles")]
        public async Task<string> GetVehiclesAsync([FromBody] StdGetVehiclesRQ dto)
        {
            await _carDomain.GetVehiclesAsync(dto);
            return "";
        }
    }
}