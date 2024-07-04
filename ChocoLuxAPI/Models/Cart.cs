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

    }
}

