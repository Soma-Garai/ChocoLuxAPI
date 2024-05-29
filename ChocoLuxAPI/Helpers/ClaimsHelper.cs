using ChocoLuxAPI.DTO;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace ChocoLuxAPI.Helpers
{
    public static class ClaimsHelper
    {
        public static void GetPermissions(this List<RoleClaimsDto> allPermissions, Type policy, string roleId)
        {
            //This line retrieves all public static fields(permissions) from the specified policy type.
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            //This loop iterates over each field (permission) retrieved from the policy type.
            foreach (FieldInfo fi in fields)
            {
                allPermissions.Add(new RoleClaimsDto { Value = fi.GetValue(null).ToString(), Type = "Permissions" });
            }
        }
        //this extension method is responsible for adding the selected claims from the UI to the user role.
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
