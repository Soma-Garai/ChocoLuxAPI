

using ChocoLuxAPI.DTO;

namespace ChocoLuxAPI.Models
{
    public class CartItem
    {
        public Guid CartItemId { get; set; } 
        public int Quantity { get; set; }    // Quantity of the product

        public ProductDto Product { get; set; } // The product in the cart
    }
}
