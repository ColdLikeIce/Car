using System.Net;
using System;
using System.Net.Http.Headers;
using System.Text;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Share;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;
using Twilio.Jwt.AccessToken;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace HeyTripCarWeb.Supplier
{
    public class TestApplyPolicy
    {
        public static async Task TestApplyPolicyAsync()
        {
            string xmlStr = @"<?xml version=""1.0""?>
<TXTpaic>
    <TXTpaicRequest>
        <Policy>
            <transApplDate>2023-08-17 09:54:11</transApplDate>
            <count>1</count>
            <transBeginDate>2023-08-18 00:00:00</transBeginDate>
            <transEndDate>2023-08-18 23:59:59</transEndDate>
            <Party>
                <partyTypeCode>1</partyTypeCode>
                <relApplicant>1</relApplicant>
                <roleTypeCode>001</roleTypeCode>
                <healthInfoFlag>1</healthInfoFlag>
                <birthDate>1991-01-01</birthDate>
                <certiType>01</certiType>
                <certiNo>440701199101010118</certiNo>
                <fullName>淖一</fullName>
                <gender>1</gender>
                <insuredID>1</insuredID>
            </Party>
            <Party>
                <partyTypeCode>1</partyTypeCode>
                <roleTypeCode>002</roleTypeCode>
                <healthInfoFlag>1</healthInfoFlag>
                <birthDate>1991-01-01</birthDate>
                <certiType>01</certiType>
                <certiNo>440701199101010118</certiNo>
                <fullName>淖一</fullName>
                <gender>1</gender>
            </Party>
            <planCode>889</planCode>
            <productCode>889150</productCode>
            <currencyCode>01</currencyCode>
            <paymentMode>0</paymentMode>
            <agentCode>agent81867</agentCode>
        </Policy>
        <transType>025</transType>
        <transRefId>R_UW_0692a9ca0db24245bd8b4c34a9382706</transRefId>
        <transExeDate>2023-08-17 09:54:11</transExeDate>
    </TXTpaicRequest>
</TXTpaic>";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 设置请求头部
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/html"));
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                    var byteArray = new System.Text.UTF8Encoding().GetBytes("avaiTest:password");
                    var token = Convert.ToBase64String(byteArray);
                    // 设置Authorization头部为Bearer token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var content = new StringContent(xmlStr, System.Text.Encoding.GetEncoding("GBK"), "text/xml");
                    HttpResponseMessage response = await client.PostAsync("https://uats.axa.cn/webservice/slNonMotor.do", content);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine((int)response.StatusCode + "\n" + responseBody);
                }
                catch (Exception ex)
                {
                    Log.Error($"请求发生错误{ex.Message}");
                }
                finally
                {
                }
            }
        }
    }
}