using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        //To show the user with their assigned roles in a table
        [Authorize]
        [HttpGet("UserAndTheirRolesList")]
        public async Task<IActionResult> UserAndTheirRolesList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesDto>();

            foreach (UserModel user in users)
            {
                // Check if the user has the "Admin" role
                //var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                //if (!isAdmin)
                //{
                    var thisViewModel = new UserRolesDto();
                    thisViewModel.UserId = user.Id;
                    thisViewModel.UserName= user.UserName;
                    thisViewModel.Email = user.Email;
                    thisViewModel.FirstName = user.FirstName;
                    thisViewModel.LastName = user.LastName;
                    thisViewModel.Roles = await GetUserRoles(user); // gets list of role names associated with the user
                    userRolesViewModel.Add(thisViewModel);
                //}
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

        [Authorize]
        [HttpGet("ManageRoles/{userId}/{userName}")]
        //to modify/manage roles of each user
        public async Task<IActionResult> ManageRoles(string userId ,string userName)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //logged in user id
            if (loggedInUserId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
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
                    UserId = userId,    //userId of the user whose manage button is clicked.
                    UserName=userName,
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                var roleExists = await _userManager.IsInRoleAsync(user, role.Name);
                if(roleExists == true) //this is giving error.
                {
                    userRolesViewModel.Selected = roleExists;
                }
                //else
                //{
                //    userRolesViewModel.Selected = false;
                //}
                model.Add(userRolesViewModel);
            }           
            return Ok(model);
        }

        //UPDATE method for user role
        [HttpPut("ManageRoles/{userId}")]
        public async Task<IActionResult> UpdateManageRoles(string userId, List<ManageUserRolesDto> model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //logged in user id
            if (loggedInUserId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
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

            return Ok("The user is updated with the Role/s");
        }

    }
}
