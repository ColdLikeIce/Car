using HeyTripCarWeb.Share;

namespace HeyTripCarWeb.Supplier.ABG
{
    public interface IABGApi : ICarSupplierApi
    {
        Task FTPInit();
    }
}