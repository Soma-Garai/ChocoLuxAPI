using ChocoLuxAPI.Models;
using ChocoLuxAPI.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace ChocoLuxAPI.Permission
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement> 
    {
        private readonly ILogger<PermissionHandler> _logger;
        public PermissionHandler(ILogger<PermissionHandler> logger)
        {
            _logger = logger;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            
            var userClaims = context.User.Claims; // provides access to all claims(role & user) associated with the user.
            // Log the user's claims
            _logger.LogInformation("User Claims:");
            foreach (var claim in userClaims)
            {
                _logger.LogInformation($"Type: {claim.Type}, Value: {claim.Value}");
            }
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null)
            {
                return;
            }
            var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            _logger.LogInformation(token.ToString());
            var issuer = jwtToken.Issuer;

            // Check if the user has a permission claim for the controller and any of the actions
            if (requirement.ActionNames.Any(action => userClaims.Any(c => c.Type == "Permission" &&
                                                                          c.Value == $"{requirement.ControllerName} - {action}" &&
                                                                          c.Issuer == issuer)))
            {
                context.Succeed(requirement);
                //return;
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


//public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
//{
//    private readonly UserManager<UserModel> _userManager;
//    private readonly RoleManager<IdentityRole> _roleManager;
//    private readonly ILogger<PermissionHandler> _logger;
//    public PermissionHandler(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, ILogger<PermissionHandler> logger)
//    {
//        _userManager = userManager;
//        _roleManager = roleManager;
//        _logger = logger;
//    }

//    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
//    {
//        // Get the current user from the context
//        var user = await _userManager.GetUserAsync(context.User);
//        _logger.LogInformation("User: {User}", user);

//        if (user == null)
//        {
//            _logger.LogWarning("User not found.");
//            return;
//        }

//        // Get the roles of the user
//        var userRoles = await _userManager.GetRolesAsync(user);
//        _logger.LogInformation("User roles: {Roles}", userRoles);

//        foreach (var role in userRoles)
//        {
//            // For each role, get the claims associated with the role
//            var roleClaims = await _roleManager.GetClaimsAsync(await _roleManager.FindByNameAsync(role));
//            _logger.LogInformation("Role: {Role}, Claims: {Claims}", role, roleClaims);

//            // Check if any of the role claims match the required permission
//            foreach (var action in requirement.ActionNames)
//            {
//                var requiredPermission = $"{requirement.ControllerName}-{action}";
//                if (roleClaims.Any(rc => rc.Type == "Permission" && rc.Value == requiredPermission))
//                {
//                    _logger.LogInformation("Permission granted: {Permission}", requiredPermission);
//                    context.Succeed(requirement);
//                    return;
//                }
//            }
//        }

//        _logger.LogWarning("Permission denied.");
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