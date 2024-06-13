using Serilog;
using System.Net;

namespace CarrentalWeb.Util
{
    public class HttpHelper
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
                Log.Error($"http异常{ex.Message}");
            }
            return responseText;
        }
    }
}