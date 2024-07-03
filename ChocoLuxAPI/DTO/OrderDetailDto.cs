namespace ChocoLuxAPI.DTO
{
    public class OrderDetailDto
    {
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public int? ProductPrice { get; set; }
        public int? TotalPrice { get; set; }
    }
}
