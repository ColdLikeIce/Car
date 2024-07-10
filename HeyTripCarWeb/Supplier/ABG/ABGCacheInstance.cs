using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;

namespace HeyTripCarWeb.Supplier.ABG
{
    public class ABGCacheInstance
    {
        private static readonly object _locker = new Object();
        private static ABGCacheInstance _instance = null;
        private static List<CarLocationSupplier> allLocation = new List<CarLocationSupplier>();
        private static List<AbgYoungDriver> YoungDriverList = new List<AbgYoungDriver>();

        /// <summary>
        /// 单例
        /// </summary>
        public static ABGCacheInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ABGCacheInstance();
                        }
                    }
                }
                return _instance;
            }
        }

        // 获取队列中的所有元素（快照）
        public List<CarLocationSupplier> GetAllItems()
        {
            return allLocation;
        }

        public void SetLocation(List<CarLocationSupplier> locList)
        {
            allLocation = locList;
        }

        public List<AbgYoungDriver> GetAllAbgYoungDriver()
        {
            return YoungDriverList;
        }

        public void SetYoungDriver(List<AbgYoungDriver> locList)
        {
            YoungDriverList = locList;
        }
    }
}