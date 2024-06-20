using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehCancelRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehCancelRS
    {
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }

        [XmlAttribute("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute("Target")]
        public string Target { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlElement("Success")]
        public Success Success { get; set; }

        [XmlElement("VehCancelRSCore")]
        public VehCancelRSCore CancelRSCore { get; set; }

        [XmlElement("VehCancelRSInfo")]
        public VehCancelRSInfo CancelRSInfo { get; set; }
    }

    public class VehCancelRSCore
    {
        [XmlAttribute("CancelStatus")]
        public string CancelStatus { get; set; }
    }

    public class VehCancelRSInfo
    {
        [XmlElement("VehReservation")]
        public VehReservation VehReservation { get; set; }
    }
}