using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dbs;
using HeyTripCarWeb.Share.Dtos;

namespace HeyTripCarWeb.Supplier.Sixt
{
    public class SixtCacheInstance
    {
        private static readonly object _locker = new Object();
        private static SixtCacheInstance _instance = null;
        private static List<CarLocationSupplier> allLocation = new List<CarLocationSupplier>();
        private static SixtToken token = new SixtToken();

        /// <summary>
        /// 单例
        /// </summary>
        public static SixtCacheInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new SixtCacheInstance();
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

        public SixtToken GetToken()
        {
            return token;
        }

        public void SetToken(SixtToken newtoken)
        {
            token = newtoken;
        }
    }

    public class SixtToken
    {
        public string token { get; set; }
        public DateTime passtime { get; set; }
    }
}