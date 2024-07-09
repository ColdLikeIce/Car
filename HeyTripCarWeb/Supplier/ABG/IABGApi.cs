using HeyTripCarWeb.Share;

namespace HeyTripCarWeb.Supplier.ABG
{
    public interface IABGApi : ICarSupplierApi
    {
        Task<bool> InitLocation();

        Task<bool> InitLocationOperationTimes();

        Task InitCreditCardPolicy();

        Task InitYoungDriver();

        Task FTPInit();
    }
}