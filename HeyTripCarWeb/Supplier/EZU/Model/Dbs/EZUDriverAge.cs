using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.EZU.Model.Dbs
{
    [Table("EZU_DriverAge")]
    public class EZUDriverAge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? AgeId { get; set; }
        public int? DriverAge { get; set; }
        public string? IsDefault { get; set; }
    }
}