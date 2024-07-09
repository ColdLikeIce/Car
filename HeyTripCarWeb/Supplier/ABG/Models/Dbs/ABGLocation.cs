using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dbs
{
    [Table("Abg_Location_New")]
    public class ABGLocation
    {
        // 必填字段
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }

        public string LocationCode { get; set; }  // 租车点代码

        public string LocationName { get; set; }  // 租车点名称
        public string RegionCode { get; set; }    // 地区代码

        // 可选字段
        public string RegionName { get; set; }          // 地区名称

        public string RentalType { get; set; }          // 租赁类型
        public string Latitude { get; set; }            // 纬度
        public string Longitude { get; set; }           // 经度
        public string PhoneNumber { get; set; }         // 电话号码
        public string AlternativePhoneNumber { get; set; } // 备用电话号码
        public string GeoIndicator { get; set; }        // 地理指示器
        public string OutsideReturn { get; set; }       // 是否可以营业外还车
        public string HasSkiRack { get; set; }          // 是否有滑雪架
        public string HasSnowTyres { get; set; }        // 是否有雪地轮胎
        public string HasSnowChains { get; set; }       // 是否有防滑链
        public string HasChildSeat { get; set; }        // 是否有儿童座椅
        public string HasRoofLuggage { get; set; }      // 是否有车顶行李架
        public string HasHandControl { get; set; }      // 是否有手动控制装置
        public string IsGPS { get; set; }               // 是否有 GPS
        public string AvisPreferred { get; set; }       // 是否提供 Avis 优选服务
        public string ShuttleServiceAvailable { get; set; } // 是否有穿梭巴士服务
        public string RoadServiceAvailable { get; set; } // 是否提供道路服务
        public string Email { get; set; }               // 电子邮件
        public string Address { get; set; }             // 地址
        public string City { get; set; }                // 城市
        public string Postcode { get; set; }            // 邮政编码
        public string APOCode { get; set; }             // APO 代码
        public string CollectionAvailable { get; set; } // 是否提供取车服务
        public string VendorCode { get; set; }          // 供应商代码
        public string VendorId { get; set; }            // 供应商 ID
        public string VendorName { get; set; }          // 供应商名称
        public DateTime? CreateTime { get; set; }       // 创建时间
        public DateTime? UpdateTime { get; set; }       // 更新时间
        public int IsDeleted { get; set; }              // 是否已删除
        public string OperationTimes { get; set; }
        public string hashKey { get; set; }
        public string operationtimehashkey { get; set; }
    }
}