using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dbs
{
    [Table("ABG_CreditCardPolicy")]
    public class ABG_CreditCardPolicy
    {
        /// <summary>
        /// 自增的唯一标识符
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// Avis位置代码助记符，3到5字符，通常为3字符的机场IATA代码
        /// </summary>
        public string AvisLocationCode { get; set; }

        /// <summary>
        /// 国家名称，使用英语
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// 地区名称，使用英语
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 内部Avis位置编号
        /// </summary>
        public string StationNumber { get; set; }

        /// <summary>
        /// Avis位置名称，使用英语
        /// </summary>
        public string AvisLocationName { get; set; }

        /// <summary>
        /// 车辆型号名称，使用英语
        /// </summary>
        public string VehicleModelName { get; set; }

        /// <summary>
        /// Avis字母车组代码
        /// </summary>
        public string AvisCarGroup { get; set; }

        /// <summary>
        /// 车辆SIPP代码，采用四位ACRISS格式
        /// </summary>
        public string VehicleSIPPCode { get; set; }

        /// <summary>
        /// 租车所需的信用卡数量
        /// </summary>
        public int NumCreditCardsRequired { get; set; }

        /// <summary>
        /// 始终为‘1’，表示详细记录
        /// </summary>
        public string DetailRecordIndicator { get; set; } = "1";

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? Updatetime { get; set; }
    }
}