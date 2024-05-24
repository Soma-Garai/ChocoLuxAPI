using ChocoLuxAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChocoLuxAPI.ViewModels
{
    public class ProductViewModel
    {
        public Guid product_id { get; set; }
        //public int product_id { get; set; }
        public string? product_name { get; set; }
        public string? product_description { get; set; }
        public int? product_price { get; set; }
        //public int product_Quantity { get; set; }

        //This property is for the form file upload, so it's not mapped to the database
        [NotMapped]
        public IFormFile? product_image { get; set; }
        public string? product_ImagePath { get; set; }


        public Guid CategoryId { get; set; }      // Foreign key
        public Category? category { get; set; }  // Navigation property
        public string? CategoryName { get; set; }
    }
}
