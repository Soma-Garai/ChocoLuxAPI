using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChocoLuxAPI.Services
{  
    public class TokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserModel> _userManager;
        public TokenGenerator(IConfiguration configuration, UserManager<UserModel> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(UserModel user)
        {
            if (user == null)
            {
                return string.Empty;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:secretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var authClaims = new List<Claim>
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Fetch user roles using UserManager
            var roles = await _userManager.GetRolesAsync(user);

            //the claim for role will be added only if it is not null
            if (roles != null && roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            //creating the token with JwtSecurityToken
            var token = new JwtSecurityToken(
                    issuer: _configuration["jwt:validIssuer"],
                    audience: _configuration["jwt:validAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: authClaims,
                    signingCredentials: credentials
                    );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }     
    }
}
