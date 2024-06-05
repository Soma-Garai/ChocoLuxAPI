namespace ChocoLuxAPI.Models
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        
        // Navigation property for related carts
        public List<Cart> Carts { get; set; } = new List<Cart>();
    }
}
