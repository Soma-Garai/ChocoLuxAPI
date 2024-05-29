using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesDto>();

            foreach (UserModel user in users)
            {
                // Check if the user has the "Admin" role
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    var thisViewModel = new UserRolesDto();
                    thisViewModel.UserId = user.Id;
                    thisViewModel.Email = user.Email;
                    thisViewModel.FirstName = user.FirstName;
                    thisViewModel.LastName = user.LastName;
                    thisViewModel.Roles = await GetUserRoles(user); // gets list of role names associated with the user
                    userRolesViewModel.Add(thisViewModel);
                }
            }

            // Return the list of user roles as JSON
            return Ok(userRolesViewModel);
        }

        //used in index method to get list of role names associated with the user
        private async Task<List<string>> GetUserRoles(UserModel user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
            //var roles = await _userManager.GetRolesAsync(user);
            //return roles.Where(role => role != "Admin").ToList();
        }

        [HttpGet("manage")]
        public async Task<IActionResult> Manage(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with Id = {userId} cannot be found");
            }

            var model = new List<ManageUserRolesDto>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }           
            return Ok(model);
        }

        [HttpPut("manage")]
        public async Task<IActionResult> UpdateUserRoles(string userId, List<ManageUserRolesDto> model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with Id = {userId} cannot be found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                return BadRequest("Cannot remove user existing roles");
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                return BadRequest("Cannot add selected roles to user");
            }

            return Ok();
        }

    }
}
