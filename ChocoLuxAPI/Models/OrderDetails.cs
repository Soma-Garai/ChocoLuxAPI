using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public Guid OrderItemId { get; set; }// Primary Key 
        //[Foreign Key to tblOrders]
        public Guid ProductId { get; set; } // [Foreign Key to tblProducts]
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public int? ProductPrice { get; set; }
        public int? TotalPrice { get; set; }

        [ForeignKey("Orders")]
        public Guid OrderId { get; set; }
        // Navigation property
        public Orders? Orders { get; set; }
        //public Product? Products { get; set; }
    }
}
