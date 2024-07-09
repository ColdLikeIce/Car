using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dtos
{
    public class StdVehicleExtend : StdVehicle
    {
        public ABG_OTA_VehAvailRateRQ availRateRQ { get; set; }
        public VehAvailCore veCore { get; set; }
        public SupplierInfo supplierInfo { get; set; }
    }
}