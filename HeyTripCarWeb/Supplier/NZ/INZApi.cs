using HeyTripCarWeb.Share;

namespace HeyTripCarWeb.Supplier.NZ
{
    public interface INZApi : ICarSupplierApi
    {
        Task<bool> BuildAllLocation();

        Task Test();
    }
}