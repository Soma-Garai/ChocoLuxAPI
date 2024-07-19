using ChocoLuxAPI.Constants;
using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Helpers;
using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;

        public PermissionController(RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            _roleManager = roleManager;
            _appDbContext = appDbContext;
        }
        //save the permissions in the RoleClaims table.
        [HttpPost("SaveRoleClaims")]
        public async Task<IActionResult> SaveRoleClaims([FromBody]List<RoleClaimDto> roleClaims)
        {
            if (roleClaims == null || !roleClaims.Any())
            {
                return BadRequest("No role claims provided.");
            }

            foreach (var roleClaim in roleClaims)
            {
                var newRoleClaim = new RoleClaim
                {
                    RoleId = roleClaim.RoleId,
                    ClaimType = roleClaim.ClaimType,
                    ClaimValue = roleClaim.ClaimValue
                };

                _appDbContext.RoleClaims.Add(newRoleClaim);
            }

            await _appDbContext.SaveChangesAsync();
            return Ok("Role claims saved successfully.");
        }
    }

   
}





//// This method is rendering the data for manage permission view.
//[HttpGet("{id}")]
//public async Task<IActionResult> GetPermissions(string id)
//{
//    // A new instance of PermissionViewModel is created to hold data for the view.
//    var model = new PermissionDto();
//    // A new empty list of RoleClaimsViewModel objects is created to hold all permissions.
//    var allPermissions = new List<RoleClaimsDto>();

//    // Permissions for the "Products" module are retrieved using the GeneratePermissionsForModule method from
//    // the Permissions class. Each permission is converted into a RoleClaimsViewModel object and
//    // added to the allPermissions list.
//    var productsPermissions = Permissions<Product>.GeneratePermissionsForModule("Products")
//                                   .Select(permission => new RoleClaimsDto { Value = permission })
//                                   .ToList();
//    allPermissions.AddRange(productsPermissions);

//    // Retrieve permissions for the "Order" module
//    var orderPermissions = Permissions<Orders>.GeneratePermissionsForModule("Orders")
//                                   .Select(permission => new RoleClaimsDto { Value = permission })
//                                   .ToList();
//    allPermissions.AddRange(orderPermissions);

//    // Retrieve the role based on the provided role ID
//    var role = await _roleManager.FindByIdAsync(id);
//    if (role == null)
//    {
//        // Handle the case where the role is not found
//        return NotFound($"Role with ID {id} not found.");
//    }

//    // The RoleId property of the model is set
//    model.RoleId = id;

//    // Claims associated with the retrieved role are retrieved 
//    var claims = await _roleManager.GetClaimsAsync(role);
//    // The values of these claims are extracted into a list.
//    var roleClaimValues = claims.Select(a => a.Value).ToList();

//    // The intersection of all permission values and role claim values is calculated to find
//    // which permissions are authorized for the role.
//    var authorizedClaims = allPermissions.Select(a => a.Value)
//                                         .Intersect(roleClaimValues)
//                                         .ToList();

//    // For each permission in allPermissions, if the permission's value is found in the authorizedClaims list,
//    // the Selected property of that permission in the model is set to true.
//    foreach (var permission in allPermissions)
//    {
//        if (authorizedClaims.Any(aC => aC == permission.Value))
//        {
//            permission.Selected = true;
//        }
//    }

//    // Finally, the RoleClaims property of the model is set to the list of all permissions,
//    // including their selection status.
//    model.RoleClaims = allPermissions;

//    return Ok(model);
//}

//// Once the Admin maps new Permission to a selected user and clicks the Save Button,
//// the enabled permissions are added to the Role.
//[HttpPost("update")]
//public async Task<IActionResult> Update([FromBody] PermissionDto model)
//{
//    var role = await _roleManager.FindByIdAsync(model.RoleId);
//    if (role == null)
//    {
//        return NotFound($"Role with ID {model.RoleId} not found.");
//    }

//    var claims = await _roleManager.GetClaimsAsync(role);
//    foreach (var claim in claims)
//    {
//        await _roleManager.RemoveClaimAsync(role, claim);
//    }

//    var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
//    foreach (var claim in selectedClaims)
//    {
//        await _roleManager.AddPermissionClaim(role, claim.Value); // AddPermissionClaim method is defined in ClaimsHelper
//    }

//    return Ok();
//}