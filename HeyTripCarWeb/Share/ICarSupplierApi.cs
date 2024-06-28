using CommonCore.Dependency;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Share
{
    public interface ICarSupplierApi : IScopedDependency
    {
        Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000);

        Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000);

        Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000);

        Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout = 15000);
    }
}