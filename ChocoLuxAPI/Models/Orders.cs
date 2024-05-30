using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class Orders
    {
        [Key]
        public Guid OrderId { get; set; } 
        //public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }  // Added OrderStatus property
        public int? TotalPrice { get; set; }  // Total price of the entire order
        public enum OrderStatus
        {
            Shipped,           //0
            PartiallyShipped,  //1
            Cancelled          //2
        }

        // Navigation property for order details
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
