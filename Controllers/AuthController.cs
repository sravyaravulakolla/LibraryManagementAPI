//using LibraryManagementAPI.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace LibraryManagementAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IConfiguration _config;

//        public AuthController(IConfiguration config)
//        {
//            _config = config;
//        }

//        [HttpPost("login")]
//        public IActionResult Login([FromBody] ApplicationUser model)
//        {
//            // Dummy authentication - replace with DB check
//            if (model.Username == "admin" && model.Password == "password")
//            {
//                var token = GenerateJwtToken(model.Username);
//                return Ok(new { token });
//            }
//            return Unauthorized("Invalid credentials");
//        }

//        private string GenerateJwtToken(string username)
//        {
//            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
//            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//            new Claim(ClaimTypes.Name, username),
//            new Claim(ClaimTypes.Role, "Admin")
//        };

//            var token = new JwtSecurityToken(
//                issuer: _config["Jwt:Issuer"],
//                audience: _config["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
//                signingCredentials: credentials);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
