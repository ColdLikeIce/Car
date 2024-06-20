using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RQs
{
    [XmlRoot(ElementName = "OTA_VehRetResRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehRetResRQ
    {
        [XmlAttribute(AttributeName = "TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "POS")]
        public POS POS { get; set; }

        [XmlElement(ElementName = "VehRetResRQCore")]
        public VehRetResRQCore VehRetResRQCore { get; set; }
    }

    public class VehRetResRQCore
    {
        [XmlElement(ElementName = "UniqueID")]
        public UniqueID UniqueID { get; set; }
    }

    public class UniqueID
    {
        [XmlAttribute(AttributeName = "Type")]
        public int Type { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }
}