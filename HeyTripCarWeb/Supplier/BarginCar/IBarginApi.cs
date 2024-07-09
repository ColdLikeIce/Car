using HeyTripCarWeb.Share;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.BarginCar
{
    public interface IBarginApi : ICarSupplierApi
    {
        Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000);

        Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000);

        Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000);

        Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout = 15000);

        Task<bool> BuildAllLocation();
    }
}