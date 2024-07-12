using ChocoLuxAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        //[Authorize]
        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            var roles = _roleManager.Roles.ToList();
            if (roles == null)
            {
                return NotFound("Role not found.");
            }
            //List<RoleDto> rolesDto = roles.ToList();
            return Ok(roles);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetRole()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (userId == null)
        //    {
        //        return Unauthorized("User ID not found in token.");
        //    }
        //    var role = await _roleManager.FindByIdAsync(userId);
        //    if (role == null)
        //    {
        //        return NotFound("Role not found.");
        //    }

        //    return Ok(role);
        //}

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok("Role created successfully.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }
        

        [HttpPut("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(string id, string name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            role.Name = name;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to update role.");
            }

            return Ok("Role updated successfully.");
        }       

        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to delete role.");
            }

            return Ok("Role deleted successfully.");
        }
    }
}
