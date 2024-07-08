namespace ChocoLuxAPI.DTO
{
    public class ControllerInfoDto
    {
        //public Guid Id { get; set; }
        public string? ControllerName { get; set; }
        /*public List<ActionInfoDto> ActionMethods { get; set; }*/ // List of action method names
        public List<ActionInfoDto> ActionMethods { get; set; } = new List<ActionInfoDto>(); // Initialize the list
    }
}
