using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.NZ.Model.Dbs
{
    [Table("EZU_Order")]
    public class NZUOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        public string? Reservationref { get; set; }
        public string? Reservationno { get; set; }
        public string? Reservationdocumentno { get; set; }
        public bool? Isonrequest { get; set; }
        public string? Vendor { get; set; }
        public DateTime PickUpDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public string? PickUpLocation { get; set; }
        public string? ReturnLocation { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int? DriverAge { get; set; }
        public string? CountryCode { get; set; }
        public int? VehiclecategoryId { get; set; }
        public int? VehiclecategorytypeId { get; set; }
        public int? InsuranceId { get; set; }
        public int? ExtrakmsId { get; set; }
        public string? SuppOrderStatus { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime LastModifiy { get; set; } = DateTime.Now;
    }
}