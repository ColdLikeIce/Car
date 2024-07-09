using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Share.Dbs
{
    [Table("CarRental.dbo.Car_Location_Suppliers")]
    public class CarLocationSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }

        public string LocationName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool Airport { get; set; }
        public string AirportCode { get; set; }
        public bool RailwayStation { get; set; }
        public string StateProv { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string OperationTime { get; set; }
        public string Supplier { get; set; }
        public string SuppLocId { get; set; }
        public int Vendor { get; set; }
        public string VendorLocId { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public string LocType { get; set; }
        public string UpdateTime { get; set; }
        public string CreateTime { get; set; }
        public int IsSystem { get; set; }
        public int MapLocationId { get; set; }
        public string MapRemark { get; set; }
        public int POIId { get; set; }
        public string TimeOutFeeInfo { get; set; }
        public string PickupDropoffInfo { get; set; }
    }
}