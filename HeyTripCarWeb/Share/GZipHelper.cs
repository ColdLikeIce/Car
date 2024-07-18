using Microsoft.AspNetCore.Components.Forms;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace HeyTripCarWeb.Share
{
    public class GZipHelper
    {
        /// <summary>
        /// 压缩字符
        /// </summary>
        public static string Compress(string s)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(s);
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close();
                    string ratejson = Convert.ToBase64String(outStream.ToArray());
                    return ratejson;
                }
            }
        }

        public static string DecompressString(string str)
        {
            var compressedData = Convert.FromBase64String(str);
            using (MemoryStream inputStream = new MemoryStream(compressedData))
            {
                using (GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        decompressionStream.CopyTo(outputStream);
                        return Encoding.UTF8.GetString(outputStream.ToArray());
                    }
                }
            }
        }

        public static string GetSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}