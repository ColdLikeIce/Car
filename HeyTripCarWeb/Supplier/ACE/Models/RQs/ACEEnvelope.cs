using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RQs
{
    // SOAP Envelope 根节点
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class ACEEnvelope
    {
        [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body { get; set; }
    }

    public class Body
    {
        [XmlElement("OTA_VehAvailRateRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
        public ACE_OTA_VehAvailRateRQ OTA_VehAvailRateRQ { get; set; }

        [XmlElement("OTA_VehRateRuleRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
        public ACE_OTA_VehRateRuleRQ ACE_OTA_VehRateRuleRQ { get; set; }

        [XmlElement("OTA_VehResRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
        public ACE_OTA_VehResRQ ACE_OTA_VehResRQ { get; set; }
    }

    public class CommonRequest
    {
        /// <summary>
        /// 1 可用性请求 2 速率请求 3 创建订单 4 取消订单
        /// </summary>
        public int? Type { get; set; }

        public ACE_OTA_VehAvailRateRQ OTA_VehAvailRateRQ { get; set; }
        public ACE_OTA_VehRateRuleRQ ACE_OTA_VehRateRuleRQ { get; set; }
        public ACE_OTA_VehResRQ ACE_OTA_VehResRQ { get; set; }
    }
}