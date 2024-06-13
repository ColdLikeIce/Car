using HeyTripCarWeb.Share;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier
{
    public class ABGApi : ICarSupplierApi
    {
        public Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            throw new NotImplementedException();
        }

        public Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout)
        {
            throw new NotImplementedException();
        }
    }
}