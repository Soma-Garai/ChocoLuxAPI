namespace ChocoLuxAPI.ViewModels
{
    public class ProductWithCategoryViewModel
    {
        public Guid product_id { get; set; }
        public string? product_name { get; set; }
        public string? product_description { get; set; }
        public int? product_price { get; set; }
        public string? product_ImagePath { get; set; }

        // Category properties
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
