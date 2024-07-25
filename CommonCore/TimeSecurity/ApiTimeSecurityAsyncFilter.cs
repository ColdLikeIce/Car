using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

using CommonCore.Security;

namespace LionAir.Core.TimeSecurity
{
    public class ApiTimeSecurityAsyncFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiTimeSecurityAsyncFilter> _logger;

        public ApiTimeSecurityAsyncFilter(IConfiguration configuration,
            ILogger<ApiTimeSecurityAsyncFilter> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.Filters.Any(filterMetadata => filterMetadata.GetType() == typeof(ApiTimeSecurityAttribute)))
            {
                return;
            }
            /*  var request = context.HttpContext.Request;
              var clientKeys = _configuration.GetSection("ClientKey").Get<List<ClientKey>>();
              if (clientKeys == null || !clientKeys.Any())
              {
                  context.Result = new ObjectResult(new AirticketsHttpResult<object> { IsSuccess = false, OrginContent = "没有配置ClientKey" });
                  return;
              }

              string? bodyJson;
              var options = new JsonSerializerOptions
              {
                  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                  PropertyNameCaseInsensitive = true
              };
              if (request.Method.ToLower().Equals("get") || request.Method.ToLower().Equals("delete"))
              {
                  var query = request.Query;
                  var parames = new Dictionary<string, object>();
                  foreach (var item in query)
                  {
                      parames.Add(item.Key, item.Value.ToString());
                  }
                  bodyJson = JsonSerializer.Serialize(parames, options);
              }
              else
              {
                  request.EnableBuffering();
                  var stream = request.Body;
                  var buffer = new byte[request.ContentLength.Value];
                  await stream.ReadAsync(buffer);
                  bodyJson = Encoding.UTF8.GetString(buffer);
                  var authrazation = JsonSerializer.Deserialize<Authrazation>(bodyJson);
                  var model = authrazation.authrazation;
                  if (string.IsNullOrWhiteSpace(model.appId))
                  {
                      context.Result = new ObjectResult(new AirticketsHttpResult<object> { IsSuccess = false, OrginContent = "签名不合法" });
                      return;
                  }
                  var client = clientKeys.FirstOrDefault(x => x.AppId == model.appId);
                  if (client == null)
                  {
                      context.Result = new ObjectResult(new AirticketsHttpResult<object> { IsSuccess = false, OrginContent = "签名不合法" });
                      return;
                  }
                  var md5 = GetMD5WithString($"{model.appId}{client.AccessKey}{model.timeSpan}");
                  if (model.token != md5)
                  {
                      context.Result = new ObjectResult(new AirticketsHttpResult<object> { IsSuccess = false, OrginContent = "签名不合法" });
                      return;
                  }
                  stream.Position = 0;
              }*/
        }

        private string GetMD5WithString(String input)
        {
            MD5 md5Hash = MD5.Create();
            // 将输入字符串转换为字节数组并计算哈希数据
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // 创建一个 Stringbuilder 来收集字节并创建字符串
            StringBuilder str = new StringBuilder();
            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
            for (int i = 0; i < data.Length; i++)
            {
                str.Append(data[i].ToString("x2")); //加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            // 返回十六进制字符串
            return str.ToString();
        }
    }

    public class Authrazation
    {
        public TokenModel authrazation { get; set; }
    }

    public class TokenModel
    {
        public string appId { get; set; }
        public string token { get; set; }
        public long timeSpan { get; set; }
    }
}