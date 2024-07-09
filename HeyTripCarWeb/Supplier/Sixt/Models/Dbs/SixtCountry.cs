using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.Sixt.Models.Dbs
{
    [Table("Sixt_Countries")]
    public class SixtCountry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}