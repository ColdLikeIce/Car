using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dbs
{
    [Table("NZ_RqLogInfo", Schema = "dbo")]
    public class NZRqLogInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [StringLength(200)]
        public string? ReqType { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(200)]
        public string? Level { get; set; }

        [Required]
        public string? Rqinfo { get; set; }

        public string? Rsinfo { get; set; }

        [StringLength(4000)]
        public string? Exception { get; set; }

        [StringLength(200)]
        public string? TheadId { get; set; }
    }
}