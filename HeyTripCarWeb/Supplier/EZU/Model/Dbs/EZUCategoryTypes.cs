using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.EZU.Model.Dbs
{
    [Table("EZU_CategoryTypes")]
    public class EZUCategoryTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? CategoryTypesId { get; set; }
        public string? VehicleCategoryType { get; set; }
        public string? DisplayOrder { get; set; }
    }
}