using CarrentalWeb.Config;
using System.Xml.Linq;

namespace CarrentalWeb.Util
{
    public class CarrentalXmlHelper
    {
        public static async Task<string> BuildXmlAndPostData(AppSetting appSetting, XElement body)
        {
            XNamespace soapEnv = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace ns = "http://wsg.avis.com/wsbang";
            // 构建完整的SOAP请求
            XDocument soapEnvelope = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(soapEnv + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "SOAP-ENV", soapEnv),
                    new XElement(soapEnv + "Header",
                        new XElement(ns + "credentials",
                            new XAttribute(XNamespace.Xmlns + "ns", "http://wsg.avis.com/wsbang/authInAny"),
                            new XElement(ns + "userID", new XAttribute(ns + "encodingType", "xsd:string"), "HeyTrip"),
                            new XElement(ns + "password", new XAttribute(ns + "encodingType", "xsd:string"), "nZPO@w_5X9)R")
                        ),
                        new XElement(ns + "WSBang-Roadmap", new XAttribute(XNamespace.Xmlns + "ns", "http://wsg.avis.com/wsbang"))
                    ),
                    new XElement(soapEnv + "Body",
                        new XElement(ns + "Request",
                            new XAttribute(XNamespace.Xmlns + "ns", "http://wsg.avis.com/wsbang"),
                            body
                        )
                    )
                )
            );
            var res = HttpHelper.PostRequest(appSetting.Url, soapEnvelope.ToString());
            return res;
        }
    }
}