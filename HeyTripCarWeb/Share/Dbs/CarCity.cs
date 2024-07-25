using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Share.Dbs
{
    [Table("car_city")]
    public class CarCity
    {
        [Key]
        public int CityId { get; set; }

        public string CityNameEn { get; set; }
    }
}