using ChocoLuxAPI.ViewModels;

namespace ChocoLuxAPI.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }    // Quantity of the product

        public ProductViewModel Product { get; set; } // The product in the cart
    }
}
