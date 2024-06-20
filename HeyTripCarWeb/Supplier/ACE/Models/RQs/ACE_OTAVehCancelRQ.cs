using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RQs
{
    [XmlRoot(ElementName = "OTA_VehCancelRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTAVehCancelRQ
    {
        [XmlAttribute(AttributeName = "TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "POS")]
        public POS POS { get; set; }

        [XmlElement(ElementName = "VehCancelRQCore")]
        public VehCancelRQCore VehCancelRQCore { get; set; }
    }

    public class VehCancelRQCore
    {
        [XmlAttribute(AttributeName = "CancelType")]
        public string CancelType { get; set; }

        [XmlElement(ElementName = "UniqueID")]
        public UniqueID UniqueID { get; set; }
    }
}