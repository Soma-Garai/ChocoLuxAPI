using ChocoLuxAPI.ViewModels;

namespace ChocoLuxAPI.Models
{
    public class CartItem
    {
        //public Guid CartItemId { get; set; }
        public Guid CartItemId = Guid.NewGuid();
        public int Quantity { get; set; }    // Quantity of the product

        public ProductViewModel Product { get; set; } // The product in the cart
    }
}
