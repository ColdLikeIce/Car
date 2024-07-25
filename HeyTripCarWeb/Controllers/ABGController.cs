using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG;
using HeyTripCarWeb.Supplier.ABG.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ABGController : ControllerBase
    {
        private readonly IABGApi _carSupplierApi;

        private readonly JwtHelper _jwtHelper;
        private readonly JwtConfig _config;

        public ABGController(IABGApi carSupplierApi, IOptions<JwtConfig> options, JwtHelper jwtHelper)
        {
            _carSupplierApi = carSupplierApi;
            _jwtHelper = jwtHelper;
            _config = options.Value;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("GetToken")]
        public ActionResult<TokenModelRes> GetTokenAsync(TokenModelQue vehicleRQ, int timeout = 15000)
        {
            string token = _jwtHelper.CreateToken(timeout, vehicleRQ.Account, vehicleRQ.Password, _config);
            return new TokenModelRes { ExpireTime = DateTime.Now.AddSeconds(timeout), Token = token };
        }

        /* [HttpPost("DecompressString")]
         public ActionResult<string> DecompressString(string body, int timeout = 15000)
         {
             return GZipHelper.DecompressString(body);
         }
 */

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

        [HttpGet("Init")]
        public async Task Init()
        {
            await _carSupplierApi.FTPInit();
        }
    }
}