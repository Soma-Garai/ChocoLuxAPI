using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Claims;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokenGenerator _tokenGenerator;

        public UserController(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, AppDbContext appDbContext, IHttpContextAccessor contextAccessor, TokenGenerator tokenGenerator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _contextAccessor = contextAccessor;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new UserModel
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
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

            // Create a new session for the authenticated user
            var session = new Session
            {
                SessionId = Guid.NewGuid(),
                UserId = user.Id, // Assuming user.Id is the user's unique identifier
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1) // Example session duration
            };

            _appDbContext.TblSession.Add(session);
            await _appDbContext.SaveChangesAsync();
            // Returns an Ok response with the generated JWT token and SessionId
            return Ok(new { Token = token, SessionId = session.SessionId });

            // Returns an Ok response with the generated JWT token
            //return Ok(token);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful.");
        }

        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                var m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
