using ChocoLuxAPI.Models;
using ChocoLuxAPI.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ChocoLuxAPI.Permission
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                    PermissionRequirement requirement)
        {
            var userClaims = context.User.Claims; // provides access to all claims(role & user) associated with the user.
                                                  // Log the user's claims
            Console.WriteLine("User Claims:");
            foreach (var claim in userClaims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            // Check if the user has a permission claim for the controller and any of the actions
            if (requirement.ActionNames.Any(action => userClaims.Any(c => c.Type == "Permission" &&
                                                                          c.Value ==
                                                                          $"{requirement.ControllerName}-{action}")))
            {
                context.Succeed(requirement);
            }

            await Task.Yield();
        }
    }
}
//protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
//            PermissionRequirement requirement)
//{
//    var userClaims = context.User.Claims;

//    // Check if the user has a permission claim for the controller and any of the actions
//    if (requirement.ActionNames.Any(action => userClaims.Any(c => c.Type == "Permission" &&
//                                                                  c.Value ==
//                                                                  $"{requirement.ControllerName}-{action}")))
//    {
//        context.Succeed(requirement);
//    }

//    await Task.Yield();
//}


//private readonly UserManager<UserModel> _userManager;
//private readonly RoleManager<IdentityRole> _roleManager;
//public PermissionHandler(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
//{
//    _userManager = userManager;
//    _roleManager = roleManager;
//}

//protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
//{
//    var user = await _userManager.GetUserAsync(context.User);
//    if (user == null)
//    {
//        return;
//    }

//    var userRoles = await _userManager.GetRolesAsync(user);
//    foreach (var role in userRoles)
//    {
//        var roleClaims = await _roleManager.GetClaimsAsync(await _roleManager.FindByNameAsync(role));
//        if (roleClaims.Any(rc => rc.Type == "Permission" && rc.Value == requirement.Permission))
//        {
//            context.Succeed(requirement);
//            return;
//        }
//    }
//}



//protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
//{
//    if (context.User == null)
//    {
//        return;
//    }
//    var permissionss = context.User.Claims.Where(x => x.Type == "Permission" &&
//                                                        x.Value == requirement.Permission;
//    ;
//    if (permissionss.Any())
//    {
//        context.Succeed(requirement);
//        return;
//    }