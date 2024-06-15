using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class CommonEnvelope
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces { get; set; } = new XmlSerializerNamespaces();

        public CommonEnvelope()
        {
            Namespaces.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
            Namespaces.Add("xsi", "http://www.w3.org/1999/XMLSchema-instance");
            Namespaces.Add("xsd", "http://www.w3.org/1999/XMLSchema");
            Namespaces.Add("ns", "http://wsg.avis.com/wsbang");
        }

        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAP_ENV_Header Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAP_ENV_Body Body { get; set; }
    }

    public class SOAP_ENV_Header
    {
        [XmlElement(ElementName = "credentials", Namespace = "http://wsg.avis.com/wsbang/authInAny")]
        public Credentials Credentials { get; set; }

        [XmlElement(ElementName = "WSBang-Roadmap", Namespace = "http://wsg.avis.com/wsbang")]
        public WSBang_Roadmap WSBangRoadmap { get; set; }
    }

    public class Credentials
    {
        [XmlElement(ElementName = "userID", Namespace = "http://wsg.avis.com/wsbang/authInAny")]
        public EncodedString UserID { get; set; }

        [XmlElement(ElementName = "password", Namespace = "http://wsg.avis.com/wsbang/authInAny")]
        public EncodedString Password { get; set; }
    }

    public class EncodedString
    {
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "encodingType")]
        public string EncodingType { get; set; }
    }

    public class WSBang_Roadmap
    {
    }

    public class SOAP_ENV_Body
    {
        [XmlElement(ElementName = "Request", Namespace = "http://wsg.avis.com/wsbang")]
        public ns_Request Request { get; set; }
    }

    public class ns_Request
    {
        [XmlElement(ElementName = "OTA_VehAvailRateRQ", Namespace = "")]
        public OTA_VehAvailRateRQ OTA_VehAvailRateRQ { get; set; }
    }
}