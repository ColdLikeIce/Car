using System.Security.Cryptography;
using System.Text;

namespace HeyTripCarWeb.Share
{
    public class Md5Helper
    {
        public static string ComputeMD5Hash(string input)
        {
            // Step 1: Create an MD5 instance
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                // 转换为16位的十六进制字符串
                // 可以通过截取前8个字节来获取16位的十六进制表示（8*2=16）
                StringBuilder sb = new StringBuilder();
                for (int i = 4; i < 12; i++) // 4到11，共8字节，转成16位十六进制
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}