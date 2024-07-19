using Microsoft.AspNetCore.Identity;

namespace ChocoLuxAPI.Models
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
