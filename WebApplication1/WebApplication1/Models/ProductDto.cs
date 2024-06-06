using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ProductDto
    {
        [Required]
        public string ProductName { get; set; } = "";

        [Required]
        public string ProductBrand { get; set; } = "";

        [Required]
        public string ProductDescription { get; set; } = "";

        [Required]
        public string ProductCategory { get; set; } = "";

        [Required]
        public decimal ProductPrice { get; set; }
        public IFormFile? ProductImage { get; set; }
    }
}
