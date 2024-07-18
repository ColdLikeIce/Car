using Dapper;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier;
using Newtonsoft.Json;

namespace HeyTripCarWeb.Share
{
    public class ApiLogHelper
    {
        /// <summary>
        /// 插入接口日志 要求异步
        /// </summary>
        /// <returns></returns>
        public static async Task<(string, DynamicParameters, string)> GetApiLogSql(LogEnum type)
        {
            var logList = SupplierLogInstance.GetLogInfoItem(type);
            if (logList.Count == 0)
            {
                return (null, null, null);
            }
            var tableName = $"{type.ToString()}_RqLogInfo";
            var sql = $"INSERT INTO {tableName} (date,theadId,reqType, level, rqinfo,rsinfo,exception) VALUES ";
            var parameters = new DynamicParameters();

            var values = new List<string>();
            int index = 0;
            foreach (var item in logList)
            {
                var date = $"@date{index}";
                var theadId = $"@theadId{index}";
                var rqtype = $"@type{index}";
                var level = $"@level{index}";
                var rqinfo = $"@rqinfo{index}";
                var rsinfo = $"@rsinfo{index}";
                var exception = $"@exception{index}";

                values.Add($"({date},{theadId},{rqtype}, {level}, {rqinfo},{rsinfo},{exception})");

                parameters.Add(date, item.Date);
                parameters.Add(theadId, item.theadId);
                parameters.Add(rqtype, item.ApiType.ToString());
                parameters.Add(level, item.Level);
                parameters.Add(rqinfo, item.rqInfo);
                //压缩一下
                var rsInfo = item.rsInfo;
                item.rsInfo = GZipHelper.Compress(rsInfo);
                parameters.Add(rsinfo, item.rsInfo);
                parameters.Add(exception, item.exception);
                index++;
            }

            sql += string.Join(", ", values);

            return (sql, parameters, JsonConvert.SerializeObject(logList));
        }
    }
}