using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    [XmlRoot("OTA_VehCancelRQ", Namespace = "http://www.w3.org/2008/XMLSchema-instance")]
    public class ABG_OTAVehCancelRQ
    {
        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlElement("POS")]
        public POS POS { get; set; }

        [XmlElement("VehCancelRQCore")]
        public VehCancelRQCore VehCancelRQCore { get; set; }

        [XmlElement("VehCancelRQInfo")]
        public VehCancelRQInfo VehCancelRQInfo { get; set; }
    }

    public class VehCancelRQCore
    {
        [XmlAttribute("CancelType")]
        public string CancelType { get; set; }

        [XmlElement("UniqueID")]
        public UniqueID UniqueID { get; set; }

        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; }
    }

    public class UniqueID
    {
        [XmlAttribute("Type")]
        public int Type { get; set; }

        [XmlAttribute("ID")]
        public string ID { get; set; }
    }

    public class VehCancelRQInfo
    {
        [XmlElement("Vendor")]
        public Vendor Vendor { get; set; }
    }

    public class Vendor
    {
        [XmlAttribute("CompanyShortName")]
        public string CompanyShortName { get; set; }
    }
}