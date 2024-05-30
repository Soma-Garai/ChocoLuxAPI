using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } 
        public string CategoryName { get; set; }
        public List<Product>? Products { get; set; } // Navigation property
        
    }
}
