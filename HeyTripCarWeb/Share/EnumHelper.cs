using Serilog;

namespace HeyTripCarWeb.Share
{
    public class EnumHelper
    {
        /// <summary>
        /// 根据枚举字符串获取枚举类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T? GetEnumTypeByStr<T>(string str)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), str);
            }
            catch (Exception ex)
            {
                Log.Error($"枚举值不匹配【{typeof(T)}】【{str}】");
                return default(T);
            }
        }
    }
}