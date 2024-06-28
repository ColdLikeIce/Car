using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    [XmlRoot(ElementName = "OTA_VehRateRuleRQ")]
    public class ABG_OTA_VehRateRuleRQ
    {
        [XmlElement(ElementName = "POS")]
        public POS POS { get; set; }

        [XmlElement(ElementName = "RentalInfo")]
        public RentalInfo RentalInfo { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "CompanyShortName")]
        public string CompanyShortName { get; set; }
    }

    [XmlRoot(ElementName = "WSBang-Roadmap", Namespace = "http://wsg.avis.com/wsbang")]
    public class WSBangRoadmap
    { }

    [XmlRoot(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Header
    {
        [XmlElement(ElementName = "credentials", Namespace = "http://wsg.avis.com/wsbang/authInAny")]
        public Credentials Credentials { get; set; }

        [XmlElement(ElementName = "WSBang-Roadmap", Namespace = "http://wsg.avis.com/wsbang")]
        public WSBangRoadmap WSBangRoadmap { get; set; }
    }

    [XmlRoot(ElementName = "PickUpLocation")]
    public class PickUpLocation
    {
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
    }

    [XmlRoot(ElementName = "ReturnLocation")]
    public class ReturnLocation
    {
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
    }

    [XmlRoot(ElementName = "VehicleInfo")]
    public class VehicleInfo
    {
        [XmlAttribute(AttributeName = "TypePref")]
        public string TypePref { get; set; }

        [XmlAttribute(AttributeName = "TransmissionPref")]
        public string TransmissionPref { get; set; }

        [XmlAttribute(AttributeName = "TransmissionType")]
        public string TransmissionType { get; set; }

        [XmlAttribute(AttributeName = "AirConditionPref")]
        public string AirConditionPref { get; set; }

        [XmlAttribute(AttributeName = "ClassPref")]
        public string ClassPref { get; set; }

        [XmlElement(ElementName = "VehType")]
        public VehType VehType { get; set; }

        [XmlElement(ElementName = "VehClass")]
        public VehClass VehClass { get; set; }

        [XmlElement(ElementName = "VehGroup")]
        public VehGroup VehGroup { get; set; }
    }

    [XmlRoot(ElementName = "CustomerID")]
    public class CustomerID
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "RentalInfo")]
    public class RentalInfo
    {
        [XmlElement(ElementName = "VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }

        [XmlElement(ElementName = "VehicleInfo")]
        public VehicleInfo VehicleInfo { get; set; }

        [XmlElement(ElementName = "RateQualifier")]
        public RateQualifier RateQualifier { get; set; }

        [XmlElement(ElementName = "CustomerID")]
        public CustomerID CustomerID { get; set; }
    }
}