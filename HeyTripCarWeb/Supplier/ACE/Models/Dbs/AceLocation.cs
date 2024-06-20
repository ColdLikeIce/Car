using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.ACE.Models.Dbs
{
    [Table("Ace_Location")]
    public class AceLocation
    {
        [Key]
        [MaxLength(50)]
        public string LocationCode { get; set; }

        [Required]
        [MaxLength(300)]
        public string LocationName { get; set; }

        [MaxLength(300)]
        public string StreetNmbr { get; set; }

        [MaxLength(300)]
        public string AddressLine { get; set; }

        [MaxLength(50)]
        public string Latitude { get; set; }

        [MaxLength(50)]
        public string Longitude { get; set; }

        [MaxLength(100)]
        public string PhoneNumber { get; set; }

        [MaxLength(50)]
        public string FaxNumber { get; set; }

        [Required]
        public bool AtAirport { get; set; }

        [MaxLength(10)]
        public string AirportCode { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }

        public int? CounterLocation { get; set; }

        [MaxLength(3000)]
        public string OperationTime { get; set; }

        [MaxLength(100)]
        public string AssocAirportLocList { get; set; }

        [Required]
        [MaxLength(2)]
        public string CountryCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string CountryName { get; set; }

        [MaxLength(100)]
        public string StateProv { get; set; }

        public int? CityId { get; set; }

        [MaxLength(50)]
        public string CityName { get; set; }

        public int? AirportId { get; set; }

        [MaxLength(3000)]
        public string ParkLocation { get; set; }

        [MaxLength(50)]
        public string Timezone { get; set; }

        [MaxLength(10)]
        public string VendorCode { get; set; }

        [MaxLength(50)]
        public string VendorId { get; set; }

        [MaxLength(100)]
        public string VendorName { get; set; }

        [MaxLength(300)]
        public string VendorLogo { get; set; }

        [MaxLength(50)]
        public string VendorLocationCode { get; set; }

        [MaxLength(50)]
        public string ExtendedLocationCode { get; set; }

        [MaxLength(50)]
        public string Remark { get; set; }

        [Required]
        public DateTime? CreateTime { get; set; }

        [Required]
        public DateTime? UpdateTime { get; set; }

        public int? PointType { get; set; }

        [MaxLength(10)]
        public string PointValue { get; set; }
    }
}