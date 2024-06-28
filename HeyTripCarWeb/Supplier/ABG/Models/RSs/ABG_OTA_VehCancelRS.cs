using System.Xml.Serialization;
using System.Xml;
using HeyTripCarWeb.Supplier.ACE.Models.RSs;

namespace HeyTripCarWeb.Supplier.ABG.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehCancelRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ABG_OTA_VehCancelRS
    {
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }

        [XmlElement(ElementName = "Success")]
        public string Success { get; set; }

        [XmlElement(ElementName = "VehCancelRSCore")]
        public VehCancelRSCore VehCancelRSCore { get; set; }

        [XmlElement(ElementName = "VehCancelRSInfo")]
        public VehCancelRSInfo VehCancelRSInfo { get; set; }

        [XmlAttribute(AttributeName = "SequenceNmbr")]
        public string SequenceNmbr { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces(
            new[] { new XmlQualifiedName("", "http://www.opentravel.org/OTA/2003/05") }
        );
    }

    public class VehCancelRSCore
    {
        [XmlAttribute(AttributeName = "CancelStatus")]
        public string CancelStatus { get; set; }
    }

    public class VehCancelRSInfo
    {
        [XmlElement(ElementName = "VehReservation")]
        public VehReservation VehReservation { get; set; }
    }
}