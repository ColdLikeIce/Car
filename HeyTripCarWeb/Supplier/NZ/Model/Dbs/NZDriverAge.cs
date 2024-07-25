using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.NZ.Model.Dbs
{
    [Table("NZ_DriverAge")]
    public class NZDriverAge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long? AgeId { get; set; }
        public int? DriverAge { get; set; }
        public string? IsDefault { get; set; }
    }
}