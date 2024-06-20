using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RQs
{
    [XmlRoot("OTA_VehLocSearchRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehLocSearchRQ
    {
        [XmlAttribute("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute("Target")]
        public string Target { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlElement("POS")]
        public POS POS { get; set; }

        [XmlElement("VehLocSearchCriterion")]
        public VehLocSearchCriterion VehLocSearchCriterion { get; set; }

        [XmlElement("Vendor")]
        public Vendor Vendor { get; set; }
    }

    public class Vendor
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }
    }

    public class VehLocSearchCriterion
    {
        [XmlElement("CodeRef")]
        public CodeRef CodeRef { get; set; }
    }

    public class CodeRef
    {
        [XmlAttribute("LocationCode")]
        public string LocationCode { get; set; }

        [XmlAttribute("CodeContext")]
        public string CodeContext { get; set; }
    }
}