
using Microsoft.AspNetCore.Identity;

namespace ChocoLuxAPI.DTO
{
    public class PermissionDto
    {
        public RoleManager<IdentityRole> RoleManager { get; set; }
        public string RoleId { get; set; }
        public IList<RoleClaimsDto> RoleClaims { get; set; }//Represents a list of RoleClaimsViewModel objects,
                                                                  //each representing a permission associated with the role.
    }
    //This class represents a view model for a single permission associated with a role.
    public class RoleClaimsDto
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}
