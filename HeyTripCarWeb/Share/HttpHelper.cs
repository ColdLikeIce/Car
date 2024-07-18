using System.Net;
using System.Text;

namespace HeyTripCarWeb.Share
{
    public class HttpHelper
    {
        private static readonly HttpClient HttpClient;

        static HttpHelper()
        {
            HttpClient = new HttpClient();
        }

        public static async Task<string> HttpPostByHeaders(string Url, string postDataStr, Dictionary<string, string> dicList, string contentType = "application/json")
        {
            //接口返回报文
            string result = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Timeout = 5000;
                if (contentType == null)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    request.ContentType = contentType;
                }
                foreach (var dic in dicList)
                {
                    request.Headers.Add(dic.Key, dic.Value);
                }

                byte[] data = Encoding.GetEncoding("utf-8").GetBytes(postDataStr);

                request.ContentLength = data.Length;

                Stream myRequestStream = request.GetRequestStream();

                myRequestStream.Write(data, 0, data.Length);
                myRequestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = await myStreamReader.ReadToEndAsync();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 发起POST同步请求
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType">application/xml、application/json、application/text、application/x-www-form-urlencoded</param>
        /// <param name="headers">填充消息头</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData, string contentType, int timeOut = 30, Dictionary<string, string>? headers = null, string cookie = "")
        {
            postData = postData ?? "";
            using (HttpContent httpContent = new StringContent(postData, Encoding.Default))
            {
                return HttpPost(url, httpContent, contentType, timeOut, headers, cookie);
            }
        }

        public static string HttpPost(string url, HttpContent postData, string contentType, int timeOut = 30, Dictionary<string, string>? headers = null, string cookie = "")
        {
            using (HttpClient httpClient = new HttpClient())
            {
                if (headers != null)
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    foreach (var header in headers)
                    {
                        if (header.Key.Contains("content-type"))
                        {
                            continue;
                        }
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                else
                {
                    httpClient.DefaultRequestHeaders.Clear();
                }
                if (!string.IsNullOrEmpty(contentType))
                    postData.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
                }

                httpClient.Timeout = new TimeSpan(0, 0, timeOut);
                HttpResponseMessage response = httpClient.PostAsync(url, postData).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}