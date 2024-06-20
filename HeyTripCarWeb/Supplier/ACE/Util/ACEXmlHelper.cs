using Serilog;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using HeyTripCarWeb.Supplier.ACE.Models.RQs;
using System.Text;
using System.Buffers;
using System.Threading;
using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using Twilio.TwiML;

namespace HeyTripCarWeb.Supplier.ACE.Util
{
    public class ACEXmlHelper
    {
        public static async Task<string> PostRequest(string url, ACEEnvelope envelope)
        {
            // Serialize the object to XML
            XmlSerializer serializer = new XmlSerializer(typeof(ACEEnvelope));
            var requestXml = "";
            using (StringWriter writer = new Utf8StringWriter())
            {
                // 使用 XmlWriterSettings 控制命名空间的定义
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true; // 忽略 XML 声明
                settings.Indent = true;
                settings.NewLineOnAttributes = false;

                using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                {
                    // 添加命名空间声明
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
                    namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

                    // 序列化对象
                    serializer.Serialize(xmlWriter, envelope, namespaces);
                }
                requestXml = writer.ToString();
            }
            // 发送 SOAP 请求
            using (HttpContent postData = new StringContent(requestXml, Encoding.UTF8))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("SOAPAction", "\"http://ota.acerentacar.com/Sample/RateService/VehRetRes\"");
                    var contentType = "text/xml";
                    if (!string.IsNullOrEmpty(contentType))
                        postData.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    HttpResponseMessage response = httpClient.PostAsync(url, postData).Result;

                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            return "";
        }

        /// <summary>
        /// 解析请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        public static T GetResponse<T>(string res)
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(res);
            // 设置命名空间管理器，因为节点有命名空间
            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("ns", "http://wsg.avis.com/wsbang");
            nsManager.AddNamespace("ota", "http://www.opentravel.org/OTA/2003/05");
            Type genericType = typeof(T);
            var nodeName = genericType.Name;
            XmlRootAttribute xmlRootAttr = (XmlRootAttribute)genericType.GetCustomAttributes(typeof(XmlRootAttribute), false).FirstOrDefault();

            // 使用XPath查询OTA_VehAvailRateRS节点
            XmlNode otaNode = xmlDoc.SelectSingleNode($"//ns:Response/ota:{xmlRootAttr.ElementName}", nsManager);
            if (otaNode == null)
            {
                Log.Information($"找不到对应的节点{nodeName}");
                throw new Exception("找不到对应的节点");
            }
            // 创建XmlSerializer对象，指定根节点类型
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            // 使用XmlNodeReader从XmlDocument中读取XmlNode
            using (XmlNodeReader reader = new XmlNodeReader(otaNode))
            {
                // 反序列化为OTA_VehAvailRateRS对象
                T result = (T)serializer.Deserialize(reader);
                return result;
            }
        }
    }
}