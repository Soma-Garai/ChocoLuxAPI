namespace ChocoLuxAPI.DTO
{
    public class ProductWithCategoryDto
    {
        public Guid ProductId { get; set; } = Guid.NewGuid();
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? ProductPrice { get; set; }
        public string? ProductImagePath { get; set; }

        // Category properties
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; }
    }
}
