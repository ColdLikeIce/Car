using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonCore.Util
{
    public class HtmlHelper
    {
        /// <summary>
        /// 正则匹配两个字符串之间内容
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public static string MidMatchString(string sourse, string startstr, string endstr)
        {
            Regex rg = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourse).Value;
        }

        public static string GetNumber(string source)
        {
            string result = Regex.Replace(source, @"[^0-9]+", "");
            return result;
        }

        public static bool IsNumer(string source)
        {
            return Regex.IsMatch(source, @"^[0-9]+$");
        }
    }
}