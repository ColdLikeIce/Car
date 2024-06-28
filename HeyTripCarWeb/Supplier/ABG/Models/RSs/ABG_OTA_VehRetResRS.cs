using HeyTripCarWeb.Supplier.ACE.Models.RSs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehRetResRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ABG_OTA_VehRetResRS
    {
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }

        [XmlElement(ElementName = "Success")]
        public Success Success { get; set; }

        [XmlElement(ElementName = "VehRetResRSCore")]
        public VehRetResRSCore VehRetResRSCore { get; set; }

        [XmlAttribute(AttributeName = "SequenceNmbr")]
        public string SequenceNmbr { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
    }

    public class VehRetResRSCore
    {
        [XmlElement(ElementName = "VehReservation")]
        public VehReservation VehReservation { get; set; }

    }
}