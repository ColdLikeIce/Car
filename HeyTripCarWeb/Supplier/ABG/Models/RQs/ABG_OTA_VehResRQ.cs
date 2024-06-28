using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    [XmlRoot("OTA_VehResRQ", Namespace = "http://www.w3.org/2008/XMLSchema-instance")]
    public class ABG_OTA_VehResRQ
    {
        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlElement("POS")]
        public POS POS { get; set; }

        [XmlElement("VehResRQCore")]
        public VehResRQCore VehResRQCore { get; set; }
    }

    public class CreateVehType
    {
        [XmlAttribute("VehicleCategory")]
        public int VehicleCategory { get; set; }
    }

    public class VehResRQCore
    {
        [XmlAttribute("Status")]
        public string Status { get; set; }

        [XmlElement("VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }

        [XmlElement("Customer")]
        public Customer Customer { get; set; }

        [XmlElement("VendorPref")]
        public VendorPref VendorPref { get; set; }

        [XmlElement("VehPref")]
        public CreateVehPref VehPref { get; set; }

        [XmlElement("RateQualifier")]
        public RateQualifier RateQualifier { get; set; }
    }

    public class CreateVehPref
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
        public CreateVehType VehType { get; set; }

        [XmlElement(ElementName = "VehClass")]
        public VehClass VehClass { get; set; }

        [XmlElement(ElementName = "VehGroup")]
        public VehGroup VehGroup { get; set; }
    }
}