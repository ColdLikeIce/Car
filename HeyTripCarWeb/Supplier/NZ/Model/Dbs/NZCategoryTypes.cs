using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HeyTripCarWeb.Supplier.NZ.Model.Dbs
{
    [Table("NZ_CategoryTypes")]
    public class NZCategoryTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int? CategoryTypesId { get; set; }
        public string? VehicleCategoryType { get; set; }
        public string? DisplayOrder { get; set; }
    }
}