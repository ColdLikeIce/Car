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