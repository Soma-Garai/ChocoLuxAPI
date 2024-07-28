using Microsoft.AspNetCore.Authorization;

namespace ChocoLuxAPI.Permission
{
    //The PermissionRequirement class represents a "custom authorization" requirement in an ASP.NET Core application.
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string ControllerName { get; }
        public List<string> ActionNames { get; }


        public PermissionRequirement(string controllerName, List<string> actionNames)
        {
            ControllerName = controllerName;
            ActionNames = actionNames ?? new List<string>();
        }
        //public string Permission { get; private set; }
        //public PermissionRequirement(string permission)
        //{
        //    Permission = permission;
        //}
    }
}
