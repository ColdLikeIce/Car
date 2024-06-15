using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using System.Xml.Serialization;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;

namespace HeyTripCarWeb.Supplier.ABG.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehAvailRateRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class OTA_VehAvailRateRS
    {
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "SequenceNmbr")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "Success")]
        public Success Success { get; set; }

        [XmlElement(ElementName = "VehAvailRSCore")]
        public VehAvailRSCore VehAvailRSCore { get; set; }
    }

    public class Success
    {
        // This class represents an empty <Success/> tag
    }

    public class VehAvailRSCore
    {
        [XmlElement(ElementName = "VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }

        [XmlElement(ElementName = "VehVendorAvails")]
        public VehVendorAvails VehVendorAvails { get; set; }
    }

    public class VehRentalCore
    {
        [XmlAttribute(AttributeName = "PickUpDateTime")]
        public DateTime PickUpDateTime { get; set; }

        [XmlAttribute(AttributeName = "ReturnDateTime")]
        public DateTime ReturnDateTime { get; set; }

        [XmlElement(ElementName = "PickUpLocation")]
        public Location PickUpLocation { get; set; }

        [XmlElement(ElementName = "ReturnLocation")]
        public Location ReturnLocation { get; set; }
    }

    public class VehVendorAvails
    {
        [XmlElement(ElementName = "VehVendorAvail")]
        public List<VehVendorAvail> VehVendorAvail { get; set; }
    }

    public class VehVendorAvail
    {
        [XmlElement(ElementName = "Vendor")]
        public string Vendor { get; set; }

        [XmlElement(ElementName = "VehAvails")]
        public VehAvails VehAvails { get; set; }
    }

    public class VehAvails
    {
        [XmlElement(ElementName = "VehAvail")]
        public List<VehAvail> VehAvail { get; set; }
    }

    public class VehAvail
    {
        [XmlElement(ElementName = "VehAvailCore")]
        public VehAvailCore VehAvailCore { get; set; }
    }

    public class VehAvailCore
    {
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "Vehicle")]
        public Vehicle Vehicle { get; set; }

        [XmlElement(ElementName = "RentalRate")]
        public RentalRate RentalRate { get; set; }

        [XmlElement(ElementName = "TotalCharge")]
        public TotalCharge TotalCharge { get; set; }
    }

    public class Vehicle
    {
        [XmlAttribute(AttributeName = "AirConditionInd")]
        public bool AirConditionInd { get; set; }

        [XmlAttribute(AttributeName = "TransmissionType")]
        public string TransmissionType { get; set; }

        [XmlElement(ElementName = "VehType")]
        public VehType VehType { get; set; }

        [XmlElement(ElementName = "VehClass")]
        public VehClass VehClass { get; set; }

        [XmlElement(ElementName = "VehMakeModel")]
        public VehMakeModel VehMakeModel { get; set; }

        [XmlElement(ElementName = "PictureURL")]
        public string PictureURL { get; set; }
    }

    public class VehMakeModel
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    public class RentalRate
    {
        [XmlElement(ElementName = "RateDistance")]
        public RateDistance RateDistance { get; set; }

        [XmlElement(ElementName = "VehicleCharges")]
        public VehicleCharges VehicleCharges { get; set; }

        [XmlElement(ElementName = "RateQualifier")]
        public RateQualifier RateQualifier { get; set; }
    }

    public class RateDistance
    {
        [XmlAttribute(AttributeName = "Unlimited")]
        public bool Unlimited { get; set; }

        [XmlAttribute(AttributeName = "DistUnitName")]
        public string DistUnitName { get; set; }

        [XmlAttribute(AttributeName = "VehiclePeriodUnitName")]
        public string VehiclePeriodUnitName { get; set; }

        [XmlAttribute(AttributeName = "Quantity")]
        public int Quantity { get; set; }
    }

    public class VehicleCharges
    {
        [XmlElement(ElementName = "VehicleCharge")]
        public List<VehicleCharge> VehicleCharge { get; set; }
    }

    public class VehicleCharge
    {
        [XmlAttribute(AttributeName = "TaxInclusive")]
        public bool TaxInclusive { get; set; }

        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "GuaranteedInd")]
        public bool GuaranteedInd { get; set; }

        [XmlAttribute(AttributeName = "IncludedInRate")]
        public bool IncludedInRate { get; set; }

        [XmlAttribute(AttributeName = "Amount")]
        public decimal Amount { get; set; }

        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }

        [XmlAttribute(AttributeName = "Purpose")]
        public string Purpose { get; set; }
    }

    public class TotalCharge
    {
        [XmlAttribute(AttributeName = "RateTotalAmount")]
        public decimal RateTotalAmount { get; set; }

        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
    }
}