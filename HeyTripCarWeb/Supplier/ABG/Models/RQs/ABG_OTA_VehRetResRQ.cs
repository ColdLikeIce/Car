using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    /// <summary>
    /// 订单详情
    /// </summary>
    [XmlRoot(ElementName = "OTA_VehRetResRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ABG_OTA_VehRetResRQ
    {
        [XmlElement(ElementName = "POS")]
        public POS POS { get; set; }

        [XmlElement(ElementName = "VehRetResRQCore")]
        public VehRetResRQCore VehRetResRQCore { get; set; }

        [XmlElement(ElementName = "VehRetResRQInfo")]
        public VehRetResRQInfo VehRetResRQInfo { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
    }

    public class VehRetResRQCore
    {
        [XmlElement(ElementName = "UniqueID")]
        public UniqueID UniqueID { get; set; }

        [XmlElement(ElementName = "PersonName")]
        public PersonName PersonName { get; set; }
    }

    public class VehRetResRQInfo
    {
        [XmlElement(ElementName = "Vendor")]
        public Vendor Vendor { get; set; }
    }
}