using HeyTripCarWeb.Share;

namespace HeyTripCarWeb.Supplier.Sixt
{
    public interface ISixtApi : ICarSupplierApi
    {
        Task<bool> BuildAllLocation();

        Task test();
    }
}