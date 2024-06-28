using HeyTripCarWeb.Share.Dtos;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HeyTripCarWeb.Share
{
    /// <summary>
    /// 供应商请求日志记录
    /// </summary>
    public class SupplierLogInstance
    {
        private static readonly object _locker = new Object();
        private static SupplierLogInstance _instance = null;
        private static ConcurrentQueue<LogInfo> _dataQueue = new ConcurrentQueue<LogInfo>();

        /// <summary>
        /// 单例
        /// </summary>
        public static SupplierLogInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new SupplierLogInstance();
                        }
                    }
                }
                return _instance;
            }
        }

        // 添加元素到队列中
        public void Enqueue(LogInfo item)
        {
            _dataQueue.Enqueue(item);
        }

        // 从队列中取出一个元素（如果队列不为空）
        public bool TryDequeue(out LogInfo item)
        {
            return _dataQueue.TryDequeue(out item);
        }

        // 获取队列中的所有元素（快照）
        public List<LogInfo> GetAllItems()
        {
            return _dataQueue.ToList();
        }

        public static List<LogInfo> GetLogInfoItem(LogEnum type)
        {
            var tempQueue = new ConcurrentQueue<LogInfo>();
            List<LogInfo> res = new List<LogInfo>();
            // 转移元素到临时队列，并跳过需要移除的元素
            while (_dataQueue.TryDequeue(out var item))
            {
                if (item.logType != type)
                {
                    tempQueue.Enqueue(item);
                }
                res.Add(item);
            }

            // 将临时队列中的元素重新添加回原队列
            while (tempQueue.TryDequeue(out var item))
            {
                _dataQueue.Enqueue(item);
            }
            return res;
        }
    }
}