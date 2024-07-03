namespace ChocoLuxAPI.DTO
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        /*public OrderStatus Status { get; set; }*/  // Added OrderStatus property
        public int? TotalPrice { get; set; }  // Total price of the entire order
        public enum OrderStatus
        {
            Shipped,           //0
            PartiallyShipped,  //1
            Cancelled          //2
        }
        public List<OrderDetailDto> OrderDetails { get; set; }

    }
}
