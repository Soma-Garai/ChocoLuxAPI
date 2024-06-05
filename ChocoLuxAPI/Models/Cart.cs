using ChocoLuxAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; }
        public string UserId { get; set; }
        public Guid SessionId { get; set; }

        // Navigation properties
        public Session Session { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        //public void AddItem(Guid productId, int quantity)
        //{
        //    var existingItem = CartItems.FirstOrDefault(item => item.ProductId == productId);

        //    if (existingItem != null)
        //    {
        //        // Update quantity if the item already exists in cart
        //        existingItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        // Add a new item to the cart
        //        CartItems.Add(new CartItem
        //        {
        //            ProductId = productId,
        //            Quantity = quantity
        //        });
        //    }

            //}
            //[HttpPost]
            //public void UpdateQuantity(ProductDto product, int quantity)
            //{
            //    //if(quantity == 0)
            //    //{
            //    //    return RedirectToAction("RemoveItem");
            //    //}
            //    var existingItem = Items.FirstOrDefault(item => item.Product.ProductId == product.ProductId);
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

            //public void RemoveItem(ProductDto product)
            //{
            //    var itemToRemove = Items.FirstOrDefault(item => item.Product.ProductId == product.ProductId);
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

