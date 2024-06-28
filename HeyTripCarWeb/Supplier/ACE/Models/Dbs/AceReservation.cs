using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.ACE.Models.Dbs
{
    [Table("ACE_CarProReservation")]
    public class AceReservation
    {
        public string OrderNo { get; set; }
        public string ReservationId { get; set; }
        public string ReservationType { get; set; }
        public DateTime? PickUpDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public string PickUpLocationCode { get; set; }
        public string ReturnLocationCode { get; set; }
        public string CodeContext { get; set; }
        public string SIPP { get; set; }
        public string TransmissionType { get; set; }
        public string AirConditionInd { get; set; }
        public string DriveType { get; set; }
        public string PassengerQuantity { get; set; }
        public string BaggageQuantity { get; set; }
        public string FuelType { get; set; }
        public string VehicleCategory { get; set; }
        public string DoorCount { get; set; }
        public string VehClass { get; set; }
        public string CarName { get; set; }
        public string PictureURL { get; set; }
        public string RateDistanceInfo { get; set; }
        public decimal? RateTotalAmount { get; set; }
        public decimal? EstimatedTotalAmount { get; set; }
        public string? CurrencyCode { get; set; }
        public string Location_Code { get; set; }
        public string ReturnLocation_Code { get; set; }
        public string Location_OtherInfo { get; set; }
        public string BirthDate { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        public string AddressInfo { get; set; }

        public string DocInfo { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public string OrderStatus { get; set; }
    }
}