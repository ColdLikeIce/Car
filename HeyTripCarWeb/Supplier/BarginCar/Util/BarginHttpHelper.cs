using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.BarginCar.Config;
using HeyTripCarWeb.Supplier.BarginCar.Model.RSs;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Security.Cryptography;
using System.Text;

namespace HeyTripCarWeb.Supplier.BarginCar.Util
{
    public class BarginHttpHelper
    {
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        private static string CreateHMACSHA256Signature(string jsonData, BarginCarAppSetting setting)
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

        public static async Task<T> BasePostRequest<T>(string request, BarginCarAppSetting setting)
        {
            try
            {
                var url = setting.url + setting.apiKey;

                string signature = CreateHMACSHA256Signature(request, setting);
                Dictionary<string, string> dicList = new Dictionary<string, string>();
                dicList.Add("signature", signature);

                var resStr = HttpHelper.HttpPost(url, request, "application/json", headers: dicList);

                var result = JsonConvert.DeserializeObject<T>(resStr);
                /*      if (result.status == "OK")
                      {
                          return result.result;
                      }
                      else
                      {
                          Log.Error($"接口请求失败{resStr}");
                      }*/

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}