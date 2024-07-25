using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.NZ.Config;
using HeyTripCarWeb.Supplier.Sixt.Config;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace HeyTripCarWeb.Supplier.NZ.Util
{
    public class NZHttpHelper
    {
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        private static string CreateHMACSHA256Signature(string jsonData, NZCarAppSetting setting)
        {
            string textToHash = jsonData;
            dynamic encoding = new System.Text.UTF8Encoding();
            HMACSHA256 myHMAC = new HMACSHA256(encoding.GetBytes(setting.sharedSecret));
            byte[] hashedBytes = myHMAC.ComputeHash(encoding.GetBytes(textToHash));
            StringBuilder strTemp = new StringBuilder(hashedBytes.Length * 2);
            string hex = null;
            foreach (byte b in hashedBytes)
            {
                hex = Conversion.Hex(b);
                if (hex.Length == 1)
                    hex = "0" + hex;
                strTemp.Append(hex);
            }
            string hash = strTemp.ToString();
            return hash;
        }

        public static async Task<(T, string)> BasePostRequest<T>(string request, NZCarAppSetting setting, ApiEnum type)
        {
            var responseText = "";
            var exception = "";
            var level = "info";
            var theadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                var url = setting.url + setting.apiKey;

                string signature = CreateHMACSHA256Signature(request, setting);
                Dictionary<string, string> dicList = new Dictionary<string, string>();
                dicList.Add("signature", signature);

                var resStr = await HttpHelper.HttpPostByHeaders(url, request, dicList: dicList);
                responseText = resStr;
                resStr = resStr.Replace("\"results\":[]", "\"results\":{}");
                var result = JsonConvert.DeserializeObject<T>(resStr);

                return (result, resStr);
            }
            catch (Exception ex)
            {
                Log.Error($"调用接口异常{ex.Message}");
                exception = ex.Message;

                throw ex;
            }
            finally
            {
                LogInfo loginfo = new LogInfo
                {
                    logType = LogEnum.NZ,
                    rqInfo = $"{request}",
                    rsInfo = responseText,
                    Level = level,
                    exception = exception,
                    Date = DateTime.Now,
                    ApiType = type,
                    theadId = theadId
                };
                SupplierLogInstance.Instance.Enqueue(loginfo);
            }
        }
    }
}