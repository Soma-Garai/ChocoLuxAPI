using ChocoLuxAPI.DTO;
using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; } // Unique identifier for each cart item
        public Guid ProductId { get; set; } // Foreign key to Product
        public int Quantity { get; set; }    // Quantity of the product
        public int? ProductPrice { get; set; }
        public Guid CartId { get; set; }  // Foreign key to Cart
        public Cart Cart { get; set; }  // Navigation property
    }
}
