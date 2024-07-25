using CommonCore.EntityFramework.Common;
using CommonCore.Mapper;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;
using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using HeyTripCarWeb.Supplier.BarginCar.Model.Dbs;
using HeyTripCarWeb.Supplier.Sixt.Models.Dbs;
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
        public LogToDbWorker(IServiceProvider serviceProvider, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var _domain = scope.ServiceProvider.GetRequiredService<IBaseRepository<CarSupplierDbContext>>();
                List<AbgRqLogInfo> abgLog = new List<AbgRqLogInfo>();
                List<BarginRqLogInfo> barginLog = new List<BarginRqLogInfo>();
                List<SixtRqLogInfo> sixtlog = new List<SixtRqLogInfo>();
                List<NZRqLogInfo> nzlog = new List<NZRqLogInfo>();
                // 遍历枚举
                foreach (LogEnum log in Enum.GetValues(typeof(LogEnum)))
                {
                    var logList = SupplierLogInstance.GetLogInfoItem(log);
                    switch (log)
                    {
                        case LogEnum.ABG:
                            foreach (var item in logList)
                            {
                                AbgRqLogInfo abgRqLogInfo = new AbgRqLogInfo()
                                {
                                    ReqType = item.ApiType.ToString(),
                                    Date = item.Date,
                                    Level = item.Level,
                                    Rqinfo = GZipHelper.Compress(item.rqInfo),
                                    Rsinfo = GZipHelper.Compress(item.rsInfo),
                                    Exception = item.exception,
                                    TheadId = item.theadId.ToString(),
                                };
                                abgLog.Add(abgRqLogInfo);
                            }
                            break;

                        case LogEnum.Bargin:
                            foreach (var item in logList)
                            {
                                BarginRqLogInfo abgRqLogInfo = new BarginRqLogInfo()
                                {
                                    ReqType = item.ApiType.ToString(),
                                    Date = item.Date,
                                    Level = item.Level,
                                    Rqinfo = GZipHelper.Compress(item.rqInfo),
                                    Rsinfo = GZipHelper.Compress(item.rsInfo),
                                    Exception = item.exception,
                                    TheadId = item.theadId.ToString(),
                                };
                                barginLog.Add(abgRqLogInfo);
                            }
                            break;

                        case LogEnum.Sixt:
                            foreach (var item in logList)
                            {
                                SixtRqLogInfo abgRqLogInfo = new SixtRqLogInfo()
                                {
                                    ReqType = item.ApiType.ToString(),
                                    Date = item.Date,
                                    Level = item.Level,
                                    Rqinfo = GZipHelper.Compress(item.rqInfo),
                                    Rsinfo = GZipHelper.Compress(item.rsInfo),
                                    Exception = item.exception,
                                    TheadId = item.theadId.ToString(),
                                };
                                sixtlog.Add(abgRqLogInfo);
                            }
                            break;

                        case LogEnum.NZ:
                            foreach (var item in logList)
                            {
                                NZRqLogInfo abgRqLogInfo = new NZRqLogInfo()
                                {
                                    ReqType = item.ApiType.ToString(),
                                    Date = item.Date,
                                    Level = item.Level,
                                    Rqinfo = GZipHelper.Compress(item.rqInfo),
                                    Rsinfo = GZipHelper.Compress(item.rsInfo),
                                    Exception = item.exception,
                                    TheadId = item.theadId.ToString(),
                                };
                                nzlog.Add(abgRqLogInfo);
                            }
                            break;
                    }
                }
                await _domain.GetRepository<AbgRqLogInfo>().BatchInsertAsync(abgLog);
                await _domain.GetRepository<BarginRqLogInfo>().BatchInsertAsync(barginLog);
                await _domain.GetRepository<SixtRqLogInfo>().BatchInsertAsync(sixtlog);
                await _domain.GetRepository<NZRqLogInfo>().BatchInsertAsync(nzlog);

                await Task.Delay(1 * 10 * 1000, stoppingToken);
            }
        }
    }
}