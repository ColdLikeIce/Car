using Azure.Core;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.Sixt.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net.Http.Headers;
using static Dapper.SqlMapper;
using System.Security.Authentication;
using System.Text;
using Microsoft.AspNetCore.SignalR;

namespace HeyTripCarWeb.Supplier.Sixt.Util
{
    public class SixtHttpHelper
    {
        private static readonly HttpClient Httpclient;

        static SixtHttpHelper()
        {
            Httpclient = new HttpClient();
        }

        public static async Task<string> GetToken(SixtAppSetting _setting)
        {
            string accessToken = "";
            // 创建HttpClient实例
            using (HttpClient client = new HttpClient())
            {
                // 设置Basic认证头部
                string credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_setting.clientId}:{_setting.clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                // 准备请求参数
                var requestBody = new FormUrlEncodedContent(new[]
                {
                  new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                // 发送POST请求
                HttpResponseMessage response = await client.PostAsync(_setting.TokenUrl, requestBody);

                // 处理响应
                if (response.IsSuccessStatusCode)
                {
                    // 解析JSON响应
                    string json = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(json);

                    // 提取访问令牌
                    accessToken = jsonResponse["access_token"].ToString();
                    Log.Information($"Access Token: {accessToken}");
                }
                else
                {
                    Log.Error($"HTTP Error: {response.StatusCode}");
                }
                return accessToken;
            }
        }

        public static async Task<T> PostData<T>(string apiUrl, string token, string postData, string method = "post", ApiEnum type = ApiEnum.None, Dictionary<string, string> headers = null)
        {
            using (HttpContent httpContent = new StringContent(postData, Encoding.Default))
            {
                var theadId = Thread.CurrentThread.ManagedThreadId;
                string responseText = "";
                string level = "Info";
                string exception = "";
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // 设置请求头部
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                        // 设置Authorization头部为Bearer token
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        if (headers != null)
                        {
                            foreach (var h in headers)
                            {
                                client.DefaultRequestHeaders.Add(h.Key, h.Value);
                            }
                        }
                        // 发送GET请求
                        HttpResponseMessage response = null;
                        if (method == "post")
                        {
                            response = await client.PostAsync(apiUrl, httpContent);
                        }
                        else if (method == "put")
                        {
                            response = await client.PutAsync(apiUrl, httpContent);
                        }

                        // 处理响应
                        if (response.IsSuccessStatusCode)
                        {
                            // 读取响应内容
                            responseText = await response.Content.ReadAsStringAsync();
                            Log.Information($"Log: {response.StatusCode}{responseText}");
                            var data = JsonConvert.DeserializeObject<T>(responseText);
                            return data;
                        }
                        else
                        {
                            responseText = await response.Content.ReadAsStringAsync();
                            Log.Information($"Log: {response.StatusCode}{responseText}");
                            var data = JsonConvert.DeserializeObject<T>(responseText);
                            return data;
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex.Message;
                        Log.Error($"请求发生错误{ex.Message}");
                    }
                    finally
                    {
                        LogInfo loginfo = new LogInfo
                        {
                            logType = LogEnum.Sixt,
                            rqInfo = $"{apiUrl}_{postData}",
                            rsInfo = responseText,
                            Level = level,
                            exception = exception,
                            Date = DateTime.Now,
                            ApiType = type,
                            theadId = theadId
                        };
                        SupplierLogInstance.Instance.Enqueue(loginfo);
                    }
                    return default(T);
                }
            }
        }

        public static async Task<(T, string)> DeleteData<T>(string apiUrl, string token, ApiEnum type = ApiEnum.None, Dictionary<string, string> headers = null, int timeout = 10000)
        {
            using (HttpClient client = new HttpClient())
            {
                var theadId = Thread.CurrentThread.ManagedThreadId;
                string responseText = "";
                string level = "Info";
                string exception = "";
                try
                {
                    // 设置请求头部
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                    // 设置Authorization头部为Bearer token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.Timeout = TimeSpan.FromMilliseconds(timeout);
                    if (headers != null)
                    {
                        foreach (var h in headers)
                        {
                            client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        }
                    }
                    // 发送GET请求
                    HttpResponseMessage response = await client.DeleteAsync(apiUrl);

                    // 处理响应
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        responseText = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<T>(responseText);
                        return (data, responseText);
                    }
                    else
                    {
                        responseText = await response.Content.ReadAsStringAsync();
                        Log.Information($"Log: {response.StatusCode}{responseText}");
                        var data = JsonConvert.DeserializeObject<T>(responseText);
                        return (data, responseText);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                    Log.Error($"请求发生错误{ex.Message}");
                }
                finally
                {
                    LogInfo loginfo = new LogInfo
                    {
                        logType = LogEnum.Sixt,
                        rqInfo = $"{apiUrl}",
                        rsInfo = responseText,
                        Level = level,
                        exception = exception,
                        Date = DateTime.Now,
                        ApiType = type,
                        theadId = theadId
                    };
                    SupplierLogInstance.Instance.Enqueue(loginfo);
                }
                return (default(T), "");
            }
        }

        public static async Task<(T, string)> GetData<T>(string apiUrl, string token, ApiEnum type = ApiEnum.None, Dictionary<string, string> headers = null, int timeout = 20000)
        {
            using (HttpClient client = new HttpClient())
            {
                var theadId = Thread.CurrentThread.ManagedThreadId;
                string responseText = "";
                string level = "Info";
                string exception = "";
                try
                {
                    // 设置请求头部
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                    // 设置Authorization头部为Bearer token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.Timeout = TimeSpan.FromMilliseconds(timeout);
                    if (headers != null)
                    {
                        foreach (var h in headers)
                        {
                            client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        }
                    }
                    // 发送GET请求
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // 处理响应
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        responseText = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<T>(responseText);
                        return (data, responseText);
                    }
                    else
                    {
                        responseText = await response.Content.ReadAsStringAsync();
                        Log.Information($"Log: {response.StatusCode}{responseText}");
                        var data = JsonConvert.DeserializeObject<T>(responseText);
                        return (data, responseText);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                    Log.Error($"请求发生错误{ex.Message}");
                }
                finally
                {
                    LogInfo loginfo = new LogInfo
                    {
                        logType = LogEnum.Sixt,
                        rqInfo = $"{apiUrl}",
                        rsInfo = responseText,
                        Level = level,
                        exception = exception,
                        Date = DateTime.Now,
                        ApiType = type,
                        theadId = theadId
                    };
                    SupplierLogInstance.Instance.Enqueue(loginfo);
                }
                return (default(T), "");
            }
        }
    }
}