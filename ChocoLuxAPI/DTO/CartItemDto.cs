namespace ChocoLuxAPI.DTO
{
    public class CartItemDto
    {
        //public ProductDto Product;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int Quantity { get; set; }
        public int? ProductPrice { get; set; }
        public int? TotalPrice;
    }
}
