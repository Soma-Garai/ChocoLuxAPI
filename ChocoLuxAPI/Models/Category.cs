using ChocoLuxAPI.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId = Guid.NewGuid();
        public string CategoryName { get; set; }
        public List<Product>? Products { get; set; } // Navigation property
        
    }
}
