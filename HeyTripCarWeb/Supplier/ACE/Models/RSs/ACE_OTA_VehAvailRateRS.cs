using HeyTripCarWeb.Supplier.ACE.Models.RQs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RSs
{
    [XmlRoot("OTA_VehAvailRateRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehAvailRateRS
    {
        [XmlAttribute]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute]
        public string Target { get; set; }

        [XmlAttribute]
        public decimal Version { get; set; }

        public Success Success { get; set; }

        public VehAvailRSCore VehAvailRSCore { get; set; }
    }

    public class Success
    { }

    public class VehAvailRSCore
    {
        public VehRentalCore VehRentalCore { get; set; }

        [XmlArrayItem("VehVendorAvail")]
        public List<VehVendorAvail> VehVendorAvails { get; set; }
    }

    public class Location
    {
        [XmlAttribute]
        public string LocationCode { get; set; }

        [XmlAttribute]
        public string CodeContext { get; set; }
    }

    public class VehVendorAvail
    {
        public Vendor Vendor { get; set; }

        [XmlArrayItem("VehAvail")]
        public List<VehAvail> VehAvails { get; set; }
    }

    public class Vendor
    {
        [XmlAttribute]
        public string CompanyShortName { get; set; }

        [XmlAttribute]
        public string TravelSector { get; set; }

        [XmlAttribute]
        public string Code { get; set; }

        [XmlText]
        public string Name { get; set; }
    }

    public class VehAvail
    {
        public VehAvailCore VehAvailCore { get; set; }

        public VehAvailInfo VehAvailInfo { get; set; }
    }

    public class VehAvailCore
    {
        [XmlAttribute]
        public string Status { get; set; }

        public Vehicle Vehicle { get; set; }
        public RentalRate RentalRate { get; set; }
        public TotalCharge TotalCharge { get; set; }
        public List<PricedEquip> PricedEquips { get; set; }

        [XmlArrayItem("Fee")]
        public List<Fee> Fees { get; set; }

        public List<PricedCoverage> PricedCoverages { get; set; }

        public Reference Reference { get; set; }
    }

    public class Coverage
    {
        [XmlAttribute]
        public string CoverageType { get; set; }

        [XmlAttribute]
        public string Code { get; set; }
    }

    public class PricedCoverage
    {
        public Coverage Coverage { get; set; }
        public VehicleCharge Charge { get; set; }
        public Deductible Deductible { get; set; }
    }

    public class Deductible
    {
        [XmlAttribute]
        public string CurrencyCode { get; set; }

        [XmlAttribute]
        public decimal Amount { get; set; }
    }

    public class PricedEquip
    {
        public Equipment Equipment { get; set; }
        public VehicleCharge Charge { get; set; }
    }

    public class Equipment
    {
        [XmlAttribute]
        public string EquipType { get; set; }
    }

    public class Vehicle
    {
        [XmlAttribute]
        public bool AirConditionInd { get; set; }

        [XmlAttribute]
        public string TransmissionType { get; set; }

        [XmlAttribute]
        public string FuelType { get; set; }

        [XmlAttribute]
        public string DriveType { get; set; }

        [XmlAttribute]
        public int PassengerQuantity { get; set; }

        [XmlAttribute]
        public int BaggageQuantity { get; set; }

        [XmlAttribute]
        public string Code { get; set; }

        [XmlAttribute]
        public string CodeContext { get; set; }

        public VehType VehType { get; set; }
        public VehClass VehClass { get; set; }
        public VehMakeModel VehMakeModel { get; set; }
        public string PictureURL { get; set; }
    }

    public class VehType
    {
        [XmlAttribute]
        public int VehicleCategory { get; set; }

        [XmlAttribute]
        public string DoorCount { get; set; }
    }

    public class VehClass
    {
        [XmlAttribute]
        public int Size { get; set; }
    }

    public class VehMakeModel
    {
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class RentalRate
    {
        public RateDistance RateDistance { get; set; }

        [XmlArrayItem("VehicleCharge")]
        public List<VehicleCharge> VehicleCharges { get; set; }

        public RateQualifier RateQualifier { get; set; }
        public RateRestrictions RateRestrictions { get; set; }
    }

    public class RateDistance
    {
        [XmlAttribute]
        public bool Unlimited { get; set; }

        [XmlAttribute]
        public string DistUnitName { get; set; }

        [XmlAttribute]
        public string VehiclePeriodUnitName { get; set; }

        [XmlAttribute]
        public int Quantity { get; set; }
    }

    public class VehicleCharge
    {
        [XmlAttribute]
        public string CurrencyCode { get; set; }

        [XmlAttribute]
        public decimal Amount { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public bool IncludedInEstTotalInd { get; set; }

        [XmlAttribute]
        public bool IncludedInRate { get; set; }

        [XmlAttribute(AttributeName = "TaxInclusive")]
        public bool TaxInclusive { get; set; }

        [XmlAttribute]
        public string Purpose { get; set; }

        public Calculation Calculation { get; set; }
    }

    public class Calculation
    {
        [XmlAttribute]
        public decimal UnitCharge { get; set; }

        [XmlAttribute]
        public string UnitName { get; set; }

        [XmlAttribute]
        public int Quantity { get; set; }

        [XmlAttribute]
        public decimal Percentage { get; set; }
    }

    public class RateQualifier
    {
        [XmlAttribute]
        public string RateCategory { get; set; }

        [XmlAttribute]
        public string RateQualifierValue { get; set; }

        [XmlAttribute]
        public string RatePeriod { get; set; }
    }

    public class RateRestrictions
    {
        [XmlAttribute]
        public bool AdvancedBookingInd { get; set; }

        [XmlAttribute]
        public bool GuaranteeReqInd { get; set; }
    }

    public class TotalCharge
    {
        [XmlAttribute]
        public decimal RateTotalAmount { get; set; }

        [XmlAttribute]
        public decimal EstimatedTotalAmount { get; set; }

        [XmlAttribute]
        public string CurrencyCode { get; set; }
    }

    public class Fee
    {
        [XmlAttribute]
        public string CurrencyCode { get; set; }

        [XmlAttribute]
        public decimal Amount { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public bool IncludedInEstTotalInd { get; set; }

        [XmlAttribute]
        public string Purpose { get; set; }

        public Calculation Calculation { get; set; }
    }

    public class VehAvailInfo
    {
        [XmlArrayItem("PaymentRule")]
        public List<PaymentRule> PaymentRules { get; set; }
    }

    public class PaymentRule
    {
        [XmlAttribute]
        public string CurrencyCode { get; set; }

        [XmlAttribute]
        public decimal Amount { get; set; }

        [XmlAttribute]
        public string RuleType { get; set; }
    }
}