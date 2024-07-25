using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.NZ.Model.Dbs
{
    [Table("NZ_Location")]
    public class NZLocation
    {
        // 必填字段
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }

        public string? LocationName { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public int? CityID { get; set; }
        public string? Address { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public bool? Airport { get; set; }
        public string? AirportCode { get; set; }
        public bool? RailwayStation { get; set; }
        public string? StateProv { get; set; }
        public string? StateCode { get; set; }
        public string? PostalCode { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? Fax { get; set; }
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }
        public string? OperationTime { get; set; }
        public int Supplier { get; set; }
        public string? SuppLocId { get; set; }
        public int? Vendor { get; set; }
        public string? VendorLocId { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public string? LocType { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? TimeOutFeeInfo { get; set; }
        public string? PickupDropoffInfo { get; set; }
    }
}