using Microsoft.AspNetCore.Identity;

namespace ChocoLuxAPI.Models
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public Guid Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
