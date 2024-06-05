using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; } 
        //public int product_id { get; set; }

        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? ProductPrice { get; set; }
        //public int product_Quantity { get; set; }

        //This property is for the form file upload, so it's not mapped to the database
        [NotMapped]
        public IFormFile? ProductImage { get; set; }
        public string? ProductImagePath { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }      // Foreign key
        public Category? Category { get; set; }  // Navigation property
        public string? CategoryName { get; set; }
    }
}
