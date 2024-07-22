namespace ChocoLuxAPI.DTO
{
    public class ControllerInfoDto
    {
        public string? ControllerName { get; set; }

        public List<ActionInfoDto> ActionMethods { get; set; } = new List<ActionInfoDto>(); // Initialize the list
    }
}
