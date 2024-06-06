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
        public TokenGenerator(IConfiguration configuration, UserManager<UserModel> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
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
            }
            //if (additionalClaims != null)
            //{
            //    authClaims.AddRange(additionalClaims);
            //}
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

        public async Task<string> GenerateCartJwt(UserModel user, List<CartItemDto> cartItems)
        {
            var cartClaim = new Claim("cart", JsonSerializer.Serialize(cartItems));
            return await GenerateToken(user/*, new List<Claim> { cartClaim }*/);
        }

        public List<CartItemDto> DecodeCartJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var cartClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "cart")?.Value;

            return cartClaim != null ? JsonSerializer.Deserialize<List<CartItemDto>>(cartClaim) : new List<CartItemDto>();
        }
    }
}
