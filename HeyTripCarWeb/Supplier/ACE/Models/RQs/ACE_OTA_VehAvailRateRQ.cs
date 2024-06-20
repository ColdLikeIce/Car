using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RQs
{
    // OTA_VehAvailRateRQ 元素
    [XmlRoot("OTA_VehAvailRateRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehAvailRateRQ
    {
        [XmlElement("POS")]
        public POS POS { get; set; }

        [XmlElement("VehAvailRQCore")]
        public VehAvailRQCore VehAvailRQCore { get; set; }

        [XmlAttribute("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute("Target")]
        public string Target { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }
    }

    public class POS
    {
        [XmlElement("Source")]
        public Source Source { get; set; }
    }

    public class Source
    {
        [XmlElement("RequestorID")]
        public RequestorID RequestorID { get; set; }
    }

    public class RequestorID
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("ID")]
        public string ID { get; set; }
    }

    public class VehAvailRQCore
    {
        [XmlElement("VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }
    }

    public class VehRentalCore
    {
        [XmlAttribute("PickUpDateTime")]
        public DateTime PickUpDateTime { get; set; }

        [XmlAttribute("ReturnDateTime")]
        public DateTime ReturnDateTime { get; set; }

        [XmlElement("PickUpLocation")]
        public PickUpLocation PickUpLocation { get; set; }

        [XmlElement("ReturnLocation")]
        public PickUpLocation ReturnLocation { get; set; }
    }

    public class PickUpLocation
    {
        [XmlAttribute("LocationCode")]
        public string LocationCode { get; set; }

        [XmlAttribute("CodeContext")]
        public string CodeContext { get; set; }
    }
}