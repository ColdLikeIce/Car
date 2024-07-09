using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Twilio.TwiML;
using static Dapper.SqlMapper;

namespace HeyTripCarWeb.Supplier.ABG.Util
{
    public class ABGXmlHelper
    {
        public static string PostRequest_new(string url, Envelope envelope, ApiEnum type, int timeout = 5000)
        {
            var nowtime = DateTime.Now.ToString("hhMMss");
            // Serialize the object to XML
            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            var requestXml = "";
            var theadId = Thread.CurrentThread.ManagedThreadId;
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
                    namespaces.Add("xsi", "http://www.w3.org/1999/XMLSchema-instance");
                    namespaces.Add("xsd", "http://www.w3.org/1999/XMLSchema");
                    namespaces.Add("ns", "http://wsg.avis.com/wsbang");

                    // 序列化对象
                    serializer.Serialize(xmlWriter, envelope, namespaces);
                }
                requestXml = writer.ToString();
            }
            Log.Information($"postApi_{theadId}_{nowtime}:请求参数【{requestXml}】");
            string responseText = string.Empty;
            string level = "Info";
            string exception = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                responseText = HttpHelper.HttpPost(url, requestXml, "Text/Xml", 5);

                Log.Information($"postApi_{theadId}_{nowtime}_type_{type}:返回结果【{responseText}】耗时{stopwatch.ElapsedMilliseconds}ms");
            }
            catch (System.Exception ex)
            {
                level = "Error";
                exception = ex.Message;
                Log.Error($"postApi_{theadId}_{nowtime}:返回异常{ex.Message}");
            }
            finally
            {
                LogInfo loginfo = new LogInfo
                {
                    tableName = "Abg_RqLogInfo",
                    logType = LogEnum.ABG,
                    rqInfo = requestXml,
                    rsInfo = responseText,
                    Level = level,
                    exception = exception,
                    Date = DateTime.Now,
                    ApiType = type,
                    theadId = theadId
                };
                SupplierLogInstance.Instance.Enqueue(loginfo);
            }
            return responseText;
        }

        public static string PostRequest(string url, Envelope envelope, ApiEnum type, int timeout = 5000)
        {
            var nowtime = DateTime.Now.ToString("hhMMss");
            // Serialize the object to XML
            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            var requestXml = "";
            var theadId = Thread.CurrentThread.ManagedThreadId;
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
                    namespaces.Add("xsi", "http://www.w3.org/1999/XMLSchema-instance");
                    namespaces.Add("xsd", "http://www.w3.org/1999/XMLSchema");
                    namespaces.Add("ns", "http://wsg.avis.com/wsbang");

                    // 序列化对象
                    serializer.Serialize(xmlWriter, envelope, namespaces);
                }
                requestXml = writer.ToString();
            }
            Log.Information($"postApi_{theadId}_{nowtime}:请求参数【{requestXml}】");
            string responseText = string.Empty;
            string level = "Info";
            string exception = "";
            HttpWebRequest request;
            WebResponse response;
            StreamReader reader;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.Timeout = timeout;
                request.Headers.Add("ContentType:Text/Xml");
                StreamWriter writer = new
                StreamWriter(request.GetRequestStream());
                writer.Write(requestXml);
                writer.Close();
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                responseText = reader.ReadToEnd();
                response.Close();
                stopwatch.Stop();
                Log.Information($"postApi_{theadId}_{nowtime}_type_{type}:返回结果【{responseText}】耗时{stopwatch.ElapsedMilliseconds}ms");
            }
            catch (WebException wex)
            {
                level = "Error";
                exception = wex.Message;
                Log.Error($"postApi_{theadId}_{nowtime}:返回异常{wex.Message}");
                response = wex.Response;
                reader = new
               StreamReader(response.GetResponseStream());
                responseText = reader.ReadToEnd();
                response.Close();
            }
            catch (System.Exception ex)
            {
                level = "Error";
                exception = ex.Message;
                Log.Error($"postApi_{theadId}_{nowtime}:返回异常{ex.Message}");
            }
            finally
            {
                LogInfo loginfo = new LogInfo
                {
                    tableName = "Abg_RqLogInfo",
                    logType = LogEnum.ABG,
                    rqInfo = requestXml,
                    rsInfo = responseText,
                    Level = level,
                    exception = exception,
                    Date = DateTime.Now,
                    ApiType = type,
                    theadId = theadId
                };
                SupplierLogInstance.Instance.Enqueue(loginfo);
            }
            return responseText;
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
            XmlRootAttribute xmlRootAttr = (XmlRootAttribute)genericType.GetCustomAttributes(typeof(XmlRootAttribute), false).FirstOrDefault();

            var nodeName = xmlRootAttr.ElementName;
            // 使用XPath查询OTA_VehAvailRateRS节点
            XmlNode otaNode = xmlDoc.SelectSingleNode($"//ns:Response/ota:{nodeName}", nsManager);
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