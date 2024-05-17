namespace ChocoLuxAPI.ViewModels
{
    public class ProductWithCategoryViewModel
    {
        public int product_id { get; set; }
        public string? product_name { get; set; }
        public string? product_description { get; set; }
        public int? product_price { get; set; }
        public string? product_ImagePath { get; set; }

        // Category properties
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
