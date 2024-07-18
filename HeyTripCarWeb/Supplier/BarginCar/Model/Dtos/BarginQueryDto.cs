using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;
using HeyTripCarWeb.Supplier.BarginCar.Model.RQs;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.BarginCar.Model.Dtos
{
    public class BarginQueryDto
    {
        public VehicleSeachRequest SourceQuery { get; set; }
        public CarLocationSupplier startLoc { get; set; }
        public CarLocationSupplier endLoc { get; set; }
        public StdGetVehiclesRQ vehicleRQ { get; set; }
    }
}