using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    [XmlRoot(ElementName = "OTA_VehAvailRateRQ")]
    public class ABG_OTA_VehAvailRateRQ
    {
        [XmlElement(ElementName = "POS")]
        public POS POS { get; set; }

        [XmlElement(ElementName = "VehAvailRQCore")]
        public VehAvailRQCore VehAvailRQCore { get; set; }

        [XmlElement(ElementName = "VehAvailRQInfo")]
        public VehAvailRQInfo VehAvailRQInfo { get; set; }

        [XmlAttribute]
        public int MaxResponses { get; set; }

        [XmlAttribute]
        public string ReqRespVersion { get; set; }

        [XmlAttribute]
        public string Version { get; set; }
    }

    public class POS
    {
        [XmlElement(ElementName = "Source")]
        public Source Source { get; set; }
    }

    // 子类 VehType
    public class VehType
    {
        [XmlAttribute("VehicleCategory")]
        public int VehicleCategory { get; set; }

        [XmlAttribute("DoorCount")]
        public int DoorCount { get; set; }
    }

    public class Source
    {
        [XmlElement(ElementName = "RequestorID")]
        public RequestorID RequestorID { get; set; }
    }

    public class RequestorID
    {
        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public int Type { get; set; }
    }

    public class VehAvailRQCore
    {
        [XmlElement(ElementName = "VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }

        [XmlElement(ElementName = "VendorPrefs")]
        public VendorPrefs VendorPrefs { get; set; }

        [XmlElement(ElementName = "VehPrefs")]
        public VehPrefs VehPrefs { get; set; }

        [XmlElement(ElementName = "DriverType")]
        public DriverType DriverType { get; set; }

        [XmlElement(ElementName = "RateQualifier")]
        public RateQualifier RateQualifier { get; set; }

        [XmlAttribute]
        public string Status { get; set; }
    }

    // 子类 RateQualifier
    public class RateQualifier
    {
        [XmlAttribute("RateCategory")]
        public string RateCategory { get; set; }

        [XmlAttribute("RateQualifier")]
        public string RateQualifierValue { get; set; }
    }

    // 子类 Location
    public class Location
    {
        [XmlAttribute("LocationCode")]
        public string LocationCode { get; set; }

        [XmlAttribute("CodeContext")]
        public string CodeContext { get; set; }
    }

    public class VendorPrefs
    {
        [XmlElement(ElementName = "VendorPref")]
        public VendorPref VendorPref { get; set; }
    }

    public class VendorPref
    {
        [XmlAttribute]
        public string CompanyShortName { get; set; }
    }

    public class VehPrefs
    {
        [XmlElement(ElementName = "VehPref")]
        public VehPref VehPref { get; set; }
    }

    public class VehPref
    {
        [XmlAttribute]
        public string AirConditionPref { get; set; }

        [XmlAttribute]
        public string ClassPref { get; set; }

        [XmlAttribute]
        public string TransmissionPref { get; set; }

        [XmlAttribute]
        public string TransmissionType { get; set; }

        [XmlAttribute]
        public string TypePref { get; set; }

        [XmlElement(ElementName = "VehType")]
        public VehType VehType { get; set; }

        [XmlElement(ElementName = "VehClass")]
        public VehClass VehClass { get; set; }
    }

    public class DriverType
    {
        [XmlAttribute]
        public int Age { get; set; }
    }

    public class VehAvailRQInfo
    {
        [XmlElement(ElementName = "Customer")]
        public Customer Customer { get; set; }
    }

    public class Customer
    {
        [XmlElement(ElementName = "Primary")]
        public Primary Primary { get; set; }
    }

    public class Primary
    {
        [XmlElement(ElementName = "CitizenCountryName")]
        public CitizenCountryName CitizenCountryName { get; set; }
    }

    public class CitizenCountryName
    {
        [XmlAttribute]
        public string Code { get; set; }
    }
}