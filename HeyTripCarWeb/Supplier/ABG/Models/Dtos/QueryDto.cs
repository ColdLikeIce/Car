using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.Dbs;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dtos
{
    /// <summary>
    /// 搜索接口必传字段
    /// </summary>
    public class QueryDto
    {
        public SupplierInfo SupplierInfo { get; set; }
        public DateTime PickUpDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public string PickUpLocationCode { get; set; }
        public string ReturnLocationCode { get; set; }

        //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、8（旅行车）或 9（皮卡）。
        public int VehicleCategory { get; set; } = 1;

        /// <summary>
        /// Size must be equal to one of the following: 1 (Mini), 2 (Subcompact), 3 (Economy), 4 (Compact), 5
        ///(Midsize), 6 (Intermediate), 7 (Standard), 8 (Fullsize), 9 (Luxury), 10 (Premium), or 11 (Minivan).
        /// </summary>
        public int Size { get; set; } = 3;

        /// <summary>
        /// 驾照年龄
        /// </summary>
        public int DriverAge { get; set; }

        public string RateCategory { get; set; } = "6";

        /// <summary>
        /// 国籍
        /// </summary>
        public string CitizenCountryCode { get; set; }

        public List<ABGLocation> LocList { get; set; }
    }
}