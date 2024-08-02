using Microsoft.AspNetCore.Identity;
using Stripe.Climate;
using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.Models
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }
        public Guid? OrderId { get; set; }  //foreign key
        public string? UserId { get; set; } //foreign key
        public DateTime PaymentDate { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
        public Orders Orders { get; set; }
        
    }
}
