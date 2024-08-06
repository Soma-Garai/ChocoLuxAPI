using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.DTO
{
    public class PaymentDetailsDto
    {
        public Guid? OrderId { get; set; }  
        public string? UserId { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? UPIID { get; set; }
        public string? BankName { get; set; }
        public int? AccountNumber { get; set; }
        public string? CustomerID { get; set; }
    }
}
