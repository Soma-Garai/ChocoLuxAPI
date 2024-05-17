using ChocoLuxAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChocoLuxAPI.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        //public void AddItem(ProductViewModel product, int quantity)
        //{
        //    var existingItem = Items.FirstOrDefault(item => item.Product.product_id == product.product_id);

        //    if (existingItem != null)
        //    {
        //        // Update quantity if the item already exists in cart
        //        existingItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        // Add a new item to the cart
        //        Items.Add(new CartItem
        //        {
        //            Product = product,
        //            Quantity = quantity
        //        });
        //    }

        //}
        //[HttpPost]
        //public void UpdateQuantity(ProductViewModel product, int quantity)
        //{
        //    //if(quantity == 0)
        //    //{
        //    //    return RedirectToAction("RemoveItem");
        //    //}
        //    var existingItem = Items.FirstOrDefault(item => item.Product.product_id == product.product_id);
        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity = quantity;
        //    }
        //    else
        //    {
        //        // If the item is not already in the cart, add it with the specified quantity
        //        Items.Add(new CartItem { Product = product, Quantity = quantity });
        //    }
        //}

        //public void RemoveItem(ProductViewModel product)
        //{
        //    var itemToRemove = Items.FirstOrDefault(item => item.Product.product_id == product.product_id);
        //    if (itemToRemove != null)
        //    {
        //        Items.Remove(itemToRemove);
        //    }
        //}

        //public void Clear()
        //{
        //    Items.Clear();
        //}
    }
}
