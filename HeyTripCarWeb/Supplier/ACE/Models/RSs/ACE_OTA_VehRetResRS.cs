using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehRetResRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehRetResRS
    {
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }

        [XmlAttribute(AttributeName = "TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Success")]
        public Success Success { get; set; }

        [XmlElement(ElementName = "VehRetResRSCore")]
        public VehRetResRSCore VehRetResRSCore { get; set; }
    }

    public class VehRetResRSCore
    {
        [XmlElement(ElementName = "VehReservation")]
        public VehReservation VehReservation { get; set; }
    }
}