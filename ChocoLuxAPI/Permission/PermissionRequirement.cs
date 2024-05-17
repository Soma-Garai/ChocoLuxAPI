using Microsoft.AspNetCore.Authorization;

namespace ChocoLuxAPI.Permission
{
    //The PermissionRequirement class represents a "custom authorization" requirement in an ASP.NET Core application.
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; private set; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
