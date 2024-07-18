using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.BarginCar.Model.Dbs
{
    [Table("BarginCar_DriverAge")]
    public class BarginDriverAge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? AgeId { get; set; }
        public int? DriverAge { get; set; }
        public string? IsDefault { get; set; }
    }
}