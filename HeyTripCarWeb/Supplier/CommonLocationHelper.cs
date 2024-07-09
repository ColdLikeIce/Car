using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dbs;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier
{
    public class CommonLocationHelper
    {
        public static async Task<(List<CarLocationSupplier>, List<CarLocationSupplier>)> GetLocaiton(StdGetVehiclesRQ vehicleRQ, List<CarLocationSupplier> locList)
        {
            List<CarLocationSupplier> startLocList = new List<CarLocationSupplier>();
            List<CarLocationSupplier> endLocList = new List<CarLocationSupplier>();
            if (vehicleRQ.PickUpLocationType == EnumCarLocationType.Airport
                 && vehicleRQ.ReturnLocationType == EnumCarLocationType.Airport)
            {
                startLocList = locList.Where(n => n.AirportCode == vehicleRQ.PickUpLocationCode).ToList();
                endLocList = locList.Where(n => n.AirportCode == vehicleRQ.ReturnLocationCode).ToList();
                //pickLoction = DatabaseHelper.GetLocatin(7, EnumCarLocationType.Airport, vehicleRQ.PickUpLocationCode);
                //returnLoction = DatabaseHelper.GetLocatin(7, EnumCarLocationType.Airport, vehicleRQ.ReturnLocationCode);
            }
            else if (vehicleRQ.PickUpLocationType == EnumCarLocationType.City
                && vehicleRQ.ReturnLocationType == EnumCarLocationType.City)
            {
                var lat = Convert.ToDouble(vehicleRQ.PickUpLocationCode.Split("_")[1]);
                var lon = Convert.ToDouble(vehicleRQ.PickUpLocationCode.Split("_")[2]);
                foreach (var loc in locList)
                {
                    try
                    {
                        //有一些不合法转不了数字
                        if (GeoHelper.IsWithinRange(lat, lon, Convert.ToDouble(loc.Latitude), Convert.ToDouble(loc.Longitude), 20))
                        {
                            startLocList.Add(loc);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (vehicleRQ.PickUpLocationCode != vehicleRQ.ReturnLocationCode)
                {
                    lat = Convert.ToDouble(vehicleRQ.ReturnLocationCode.Split("_")[1]);
                    lat = Convert.ToDouble(vehicleRQ.ReturnLocationCode.Split("_")[2]);
                    foreach (var loc in locList)
                    {
                        try
                        {
                            //有一些不合法转不了数字
                            if (GeoHelper.IsWithinRange(lat, lon, Convert.ToDouble(loc.Latitude), Convert.ToDouble(loc.Longitude), 20))
                            {
                                endLocList.Add(loc);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            return (startLocList, endLocList);
        }
    }
}