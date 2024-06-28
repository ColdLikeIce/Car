using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dbs
{
    [Table("ABG_RateCache")]
    public class ABGRateCache
    {
        // Properties matching the table columns
        public string SearchMD5 { get; set; } // Matches

        public string SearchKey { get; set; } // Matches
        public int SearchCount { get; set; } // Matches [int]
        public string RateMD5 { get; set; } // Matches
        public string RateCache { get; set; } // Matches [nvarchar](max)
        public int CanSaleCount { get; set; } // Matches [int]
        public DateTime UpdateTime { get; set; } // Matches [datetime]
        public DateTime PreUpdateTime { get; set; } // Matches [datetime]
        public DateTime ExpireTime { get; set; } // Matches [datetime]
    }
}