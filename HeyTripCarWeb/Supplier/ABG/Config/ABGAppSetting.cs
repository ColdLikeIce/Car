using XiWan.Car.BusinessShared.Enums;

namespace HeyTripCarWeb.Supplier.ABG.Config
{
    public class ABGAppSetting
    {
        /// <summary>
        /// 附近多少公里
        /// </summary>
        public int Km { get; set; } = 20;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int PassMin { get; set; }

        public string Url { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public List<SupplierInfo> SupplierInfos { get; set; }
    }

    public class SupplierInfo
    {
        public string Vendor { get; set; }
        public string DefaultIATA { get; set; }
        public FTP FTP { get; set; }
        public List<SecSupplier> secSuppliers { get; set; }
    }

    public class SecSupplier
    {
        public string Vendor { get; set; }

        public int VerdorType { get; set; }
        public string IATA { get; set; }
        public EnumCarPayType PayType { get; set; }
    }

    public class FTP
    {
        public string FTPUrl { get; set; }
        public List<string> FTPFiles { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
    }
}