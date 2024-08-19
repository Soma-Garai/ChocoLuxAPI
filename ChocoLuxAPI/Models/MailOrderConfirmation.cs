namespace ChocoLuxAPI.Models
{
    public class MailOrderConfirmation
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public List<IFormFile>? Attachments { get; set; }

        // Order details
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public int TotalPrice { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }

        public MailOrderConfirmation()
        {
            OrderDetails = new List<OrderDetailViewModel>();
        }
    }

    public class OrderDetailViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int ProductPrice { get; set; }
        public int TotalPrice { get; set; }
    }

}
