//using ChocoLuxAPI.Models;
//using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ChocoLuxAPI.Services
//{
//    public class CartService
//    {
//        private readonly string _secretKey;
//        private readonly string _issuer;
//        private readonly string _audience;

//        public CartService(IConfiguration configuration)
//        {
//            _secretKey = configuration["Jwt:Key"];
//            _issuer = configuration["Jwt:Issuer"];
//            _audience = configuration["Jwt:Audience"];
//        }

//        public string GenerateJwt(string userId, List<CartItem> cartItems)
//        {
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//            new Claim(JwtRegisteredClaimNames.Sub, userId),
//            new Claim("cart", JsonConvert.SerializeObject(cartItems)),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//        };

//            var token = new JwtSecurityToken(
//                issuer: _issuer,
//                audience: _audience,
//                claims: claims,
//                expires: DateTime.Now.AddHours(1),
//                signingCredentials: creds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        public List<CartItem> DecodeJwt(string token)
//        {
//            var handler = new JwtSecurityTokenHandler();
//            var jwtToken = handler.ReadJwtToken(token);
//            var cartClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "cart")?.Value;

//            return cartClaim != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartClaim) : new List<CartItem>();
//        }
//    }
//}
