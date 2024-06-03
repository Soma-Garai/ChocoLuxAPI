

using ChocoLuxAPI.DTO;

namespace ChocoLuxAPI.Models
{
    public class CartItem
    {
        public Guid CartItemId = Guid.NewGuid();
        public int Quantity { get; set; }    // Quantity of the product

        public ProductDto Product { get; set; } // The product in the cart
    }
}
