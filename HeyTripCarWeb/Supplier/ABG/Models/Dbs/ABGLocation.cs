using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dbs
{
    [Table("Abg_Location")]
    public class ABGLocation
    {
        [Key]
        [Required]
        [MaxLength(50)]
        public string LocationCode { get; set; } // NOT NULL, primary key

        [Required]
        [MaxLength(300)]
        public string LocationName { get; set; } // NOT NULL

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

        [MaxLength(10)]
        public string AirportCode { get; set; }

        [MaxLength(20)]
        public string AirportCityDesignatorCodeType { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }

        [MaxLength(3000)]
        public string OperationTime { get; set; }

        [Required]
        [MaxLength(2)]
        public string CountryCode { get; set; } // NOT NULL

        [MaxLength(100)]
        public string StateProv { get; set; }

        [MaxLength(50)]
        public string CityName { get; set; }

        public int? AirportId { get; set; } // Nullable int

        [MaxLength(10)]
        public string VendorCode { get; set; }

        [MaxLength(50)]
        public string VendorId { get; set; }

        [MaxLength(100)]
        public string VendorName { get; set; }

        [Required]
        public DateTime CreateTime { get; set; } // NOT NULL

        [Required]
        public DateTime UpdateTime { get; set; } // NOT NULL

        [MaxLength(5)]
        public string LDBMnemonic { get; set; }

        public int? LDBNumber { get; set; } // Nullable int

        public int? FleetOwnerLDBNumber { get; set; } // Nullable int

        [MaxLength(10)]
        public string LocationType { get; set; }

        [MaxLength(50)]
        public string LicenseeType { get; set; }

        [MaxLength(10)]
        public string LocationStatusType { get; set; }

        [MaxLength(10)]
        public string DbrLocationCode { get; set; }

        [MaxLength(5)]
        public string TestLocationType { get; set; }

        [MaxLength(10)]
        public string PreferredServiceType { get; set; }

        [MaxLength(5)]
        public string InternationalDivisionCodeType { get; set; }

        [MaxLength(10)]
        public string RegionNo { get; set; }

        [MaxLength(10)]
        public string LatLongSourceType { get; set; }

        [MaxLength(50)]
        public string StationSiteCodeType { get; set; }

        [MaxLength(20)]
        public string ContactName { get; set; }

        [MaxLength(10)]
        public string WireLocationType { get; set; }

        [MaxLength(10)]
        public string AutonationIndType { get; set; }

        [MaxLength(10)]
        public string SelfServiceInd { get; set; }

        [MaxLength(10)]
        public string SecureLotInd { get; set; }

        [MaxLength(10)]
        public string TruckIndicator { get; set; }

        [MaxLength(10)]
        public string DotComLocationType { get; set; }

        [MaxLength(10)]
        public string DistanceFromMapOrigin { get; set; }

        [MaxLength(500)]
        public string ConsolidatedHours { get; set; }

        [MaxLength(10)]
        public string GDSLocationCode { get; set; }

        [MaxLength(250)]
        public string StateCode { get; set; }

        [MaxLength(50)]
        public string VendorLocId { get; set; }
    }
}