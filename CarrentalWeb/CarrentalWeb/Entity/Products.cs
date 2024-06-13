using System.ComponentModel.DataAnnotations;

namespace CarrentalWeb.Entity
{
    public class Products
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}