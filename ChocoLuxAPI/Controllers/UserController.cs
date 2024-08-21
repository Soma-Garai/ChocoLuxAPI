using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using System.Security.Claims;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokenGenerator _tokenGenerator;
        private readonly ILogger<UserController> _logger;
        private readonly MailService _mailService;
        public UserController(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, 
            AppDbContext appDbContext, IHttpContextAccessor contextAccessor, TokenGenerator tokenGenerator, 
            ILogger<UserController> logger, MailService mailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _contextAccessor = contextAccessor;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
            _mailService = mailService;
        }

        [HttpPost("Register")]
        
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            MailAddress address = new MailAddress(model.Email);
            string userName = address.User;
            var user = new UserModel
            {
                UserName = userName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                //call the email service here.
                await _mailService.SendWelcomeEmailAsync(userName);
                return Ok("Registration successful.");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            // Attempts to find a user by their username/email using the UserManager
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest("User Not Found");
            }

            // Checks if the provided password matches the user's password
            var passwordResult = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordResult)
            {
                return BadRequest("Password Not Match");
            }

            // Generates a JWT token for the authenticated user
            var token = await _tokenGenerator.GenerateToken(user);

            await _appDbContext.SaveChangesAsync();
            // Returns an Ok response with the generated JWT token 
            return Ok(new { Token = token});
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId != null)
            {
                var user = await _userManager.GetUserAsync(User);
                await _signInManager.SignOutAsync();
            }
            _logger.LogInformation("User {UserId} logged out", userId);
            return Ok("User logged out successfully");
        }

        [Authorize]
        [HttpGet("Details")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var userRole = userRoles.FirstOrDefault(); // Assuming you want the first role

            var userDetails = new UserDetailsDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName= user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                UserRole = userRole
            };

            return Ok(userDetails);
        }



    }
}
