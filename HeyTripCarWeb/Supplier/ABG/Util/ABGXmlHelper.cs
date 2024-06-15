using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using Serilog;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using static Dapper.SqlMapper;

namespace HeyTripCarWeb.Supplier.ABG.Util
{
    public class ABGXmlHelper
    {
        public static string PostRequest(string url, string requestXml)
        {
            string responseText = string.Empty;
            HttpWebRequest request;
            WebResponse response;
            StreamReader reader;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.Headers.Add("ContentType:Text/Xml");
                StreamWriter writer = new
                StreamWriter(request.GetRequestStream());
                writer.Write(requestXml);
                writer.Close();
                response = request.GetResponse();
                reader = new
                StreamReader(response.GetResponseStream());
                responseText = reader.ReadToEnd();
                response.Close();
            }
            catch (WebException wex)
            {
                response = wex.Response;
                reader = new
               StreamReader(response.GetResponseStream());
                responseText = reader.ReadToEnd();
                response.Close();
            }
            catch (System.Exception ex)
            {
                Log.Error($"请求接口异常{ex.Message}");
            }
            return responseText;
        }

        public static string BuildRequest<T>(T model, ABGAppSetting _setting)
        {
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

            // 使用XPath查询OTA_VehAvailRateRS节点
            XmlNode otaNode = xmlDoc.SelectSingleNode("//ns:Response/ota:OTA_VehAvailRateRS", nsManager);

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