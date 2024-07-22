using ChocoLuxAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ResourceController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, RoleManager<IdentityRole> roleManager)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _roleManager = roleManager;
        }

        [HttpGet("GetControllers/{roleId}")]
        public async Task<IActionResult> GetControllers(string roleId)
        {
            // Retrieve existing role claims for the given role ID
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound($"Role with ID {roleId} not found.");
            }
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            var controllerInfos = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .GroupBy(ad => ad.RouteValues["controller"])
                .Select(g => new ControllerInfoDto
                {
                    ControllerName = g.Key,
                    ActionMethods = g.Select(ad => new ActionInfoDto 
                    {
                        ActionMethodName = ad.RouteValues["action"],
                        // Check if the roleClaims contain a claim with the formatted ClaimValue
                        IsSelected = roleClaims.Any(rc =>
                            rc.Type == "Permission" &&
                            rc.Value == $"{g.Key} - {ad.RouteValues["action"]}")
                    }).ToList()
                }).ToList();

            return Ok(controllerInfos);
        }
    }
}
