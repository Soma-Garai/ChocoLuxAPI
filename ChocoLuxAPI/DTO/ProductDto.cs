using ChocoLuxAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChocoLuxAPI.DTO
{
    public class ProductDto
    {
        public Guid ProductId = Guid.NewGuid();
        
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? ProductPrice { get; set; }
        //public int product_Quantity { get; set; }

        //This property is for the form file upload, so it's not mapped to the database
        [NotMapped]
        public IFormFile? ProductImage { get; set; }
        //public string? ProductImagePath { get; set; }


        public Guid CategoryId { get; set; }      // Foreign key
        /*public Category? category { get; set; }*/  // Navigation property
        public string? CategoryName { get; set; }
    }
}
