using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RQs
{
    /// <summary>
    /// 请求rateRule规则实体
    /// </summary>
    [XmlRoot(ElementName = "OTA_VehRateRuleRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehRateRuleRQ
    {
        [XmlAttribute]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute]
        public string Target { get; set; }

        [XmlAttribute]
        public string Version { get; set; }

        public POS POS { get; set; }

        public Reference Reference { get; set; }
    }

    public class Reference
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string ID { get; set; }
    }
}