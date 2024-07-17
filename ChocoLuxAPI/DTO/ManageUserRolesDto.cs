namespace ChocoLuxAPI.DTO
{
    public class ManageUserRolesDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
