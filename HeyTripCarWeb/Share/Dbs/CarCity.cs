using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Share.Dbs
{
    [Table("CarRental.dbo.car_city")]
    public class CarCity
    {
        public int CityId { get; set; }
        public string CityNameEn { get; set; }
    }
}