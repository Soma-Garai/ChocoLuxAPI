using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ChocoLuxAPI.Services
{  
    public class TokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserModel> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TokenGenerator(IConfiguration configuration, UserManager<UserModel> userManager, AppDbContext appDbContext, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _roleManager = roleManager;
        }

        private async Task<List<Claim>> GetRoleClaimsAsync(List<string> roleNames)
        {
            var roleClaims = new List<Claim>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    roleClaims.AddRange(claims);
                }
            }

            return roleClaims;
        }

        [HttpPost]
        public async Task<string> GenerateToken(UserModel user /*List<Claim> additionalClaims=null*/)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
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

                // Fetch role claims using RoleManager
                var roleClaims = await GetRoleClaimsAsync(roles.ToList());

                foreach (var roleClaim in roleClaims)
                {
                    if (roleClaim.Type == "Permission")
                    {
                        authClaims.Add(roleClaim);
                    }
                }
            }
            //creating the token with JwtSecurityToken
            var token = new JwtSecurityToken(
                    issuer: _configuration["jwt:validIssuer"],
                    audience: _configuration["jwt:validAudience"],
                    expires: DateTime.Now.AddHours(12),
                    claims: authClaims,
                    signingCredentials: credentials
                    );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
