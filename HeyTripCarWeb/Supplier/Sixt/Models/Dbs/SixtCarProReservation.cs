using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.Sixt.Models.Dbs
{
    [Table("Sixt_CarProReservation")]
    public class SixtCarProReservation
    {
        [Key]
        public string? OrderNo { get; set; }

        public string? ReservationId { get; set; }
        public string? DisplayId { get; set; }
        public string? Status { get; set; }
        public string? State { get; set; }
        public string? PointOfSale { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Vehicle { get; set; } // Assuming this stores JSON or serialized object
        public string? Drivers { get; set; } // Assuming this stores JSON or serialized object
        public DateTime? PickupDate { get; set; }
        public string? PickupStation { get; set; } // Assuming this stores JSON or serialized object
        public DateTime? ReturnDate { get; set; }
        public string? ReturnStation { get; set; } // Assuming this stores JSON or serialized object
        public string? Delivery { get; set; } // Assuming this stores JSON or serialized object
        public string? Collection { get; set; } // Assuming this stores JSON or serialized object
        public string? OfferConfiguration { get; set; } // Assuming this stores JSON or serialized object
        public string? SelfService { get; set; } // Assuming this stores JSON or serialized object
        public string? ReferenceFields { get; set; } // Assuming this stores JSON or serialized object
        public string? BonusProgram { get; set; } // Assuming this stores JSON or serialized object
        public string? CustomerType { get; set; }
        public string? CorporateCustomerNumber { get; set; }
        public string? RateCode { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}