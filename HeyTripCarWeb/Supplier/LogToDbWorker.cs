using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;

namespace HeyTripCarWeb.Supplier.ABG.Worker
{
    public class LogToDbWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceProvider"></param>
        public LogToDbWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var _domain = scope.ServiceProvider.GetRequiredService<IRepository<ABGRateCache>>();
                // 遍历枚举
                foreach (LogEnum log in Enum.GetValues(typeof(LogEnum)))
                {
                    // 打印枚举名称和数值
                    var (sql, para, list) = await ApiLogHelper.GetApiLogSql(log);
                    try
                    {
                        if (sql != null)
                        {
                            await _domain.ExecuteSql(sql, para);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"处理日志失败{ex.Message}【{list}】");
                    }
                }
                await Task.Delay(1 * 10 * 1000, stoppingToken);
            }
        }
    }
}