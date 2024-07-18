using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.BarginCar.Model.Dbs
{
    [Table("BarginCar_CategoryTypes")]
    public class BarginCategoryTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? CategoryTypesId { get; set; }
        public string? VehicleCategoryType { get; set; }
        public string? DisplayOrder { get; set; }
    }
}