namespace ChocoLuxAPI.DTO
{
    public class CartOperationDto
    {
        public Guid? SessionId { get; set; }
        public string UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }
}
