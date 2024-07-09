using Serilog;
using System.Diagnostics;

namespace HeyTripCarWeb.Supplier.ABG.Worker
{
    public class SFTPToDbWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceProvider"></param>
        public SFTPToDbWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Hour != 0)
                {
                    await Task.Delay(1 * 3600 * 1000, stoppingToken);
                    return;
                }
                using var scope = _serviceProvider.CreateAsyncScope();
                var _domain = scope.ServiceProvider.GetRequiredService<IABGApi>();
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    stopwatch.Start();
                    //下载文件
                    await _domain.FTPInit();

                    await Task.Delay(1 * 3600 * 1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Log.Error($"SFTPToDbWorker运行出错{ex.Message}");
                    continue;
                }
            }
        }
    }
}