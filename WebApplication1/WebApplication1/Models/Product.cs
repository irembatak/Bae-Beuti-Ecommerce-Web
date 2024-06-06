using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductBrand { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public string ProductCategory { get; set; } = "";

        [Precision(16, 2)]
        public decimal ProductPrice { get; set; }

       public string ProductImageName  { get; set; } = "";



    }
}
