using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using POSTaskAPI.Data;
using POSTaskAPI.DTO;
using POSTaskAPI.Helper;
using POSTaskAPI.Models;
using POSTaskAPI.RepositoryInterface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace POSTaskAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly AppDBContext db;
        private readonly JWTSetting jwtSetting;
        private readonly IGenericRepository<User> userRepo;
        private readonly IConfiguration configuration;
        private readonly PasswordHasher<User> hasher;

        public UserController(IGenericRepository<User> _userRepo, IOptions<JWTSetting> _jwtSetting, IConfiguration _configuration)
        {
            userRepo = _userRepo;
            jwtSetting = _jwtSetting.Value;
            configuration = _configuration;
            hasher = new PasswordHasher<User>();
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest userObj)
        {
            try
            {
                if (userObj == null)
                    return BadRequest();

                var userList = await userRepo.GetAllAsync();
                var user = userList.FirstOrDefault(x => x.Username == userObj.Username);
                if (user == null)
                    return NotFound(new { Message = "User Not Found!" });
                else if (!VerifyPassword(user,user.Password, userObj.Password))
                    return BadRequest(new { Message = "Incorrect Password" });

                user.Token = CreateJwt(userObj);
                await userRepo.UpdateAsync(user, user.Id);
                return Ok(new TokenResponse() { Token = user.Token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        //Not required for task but add for testing purpose
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest userObj)
        {
            try
            {
                if (userObj == null)
                    return BadRequest();

                if (await userRepo.AnyAsync("Username", userObj.Username))
                    return BadRequest(new { Message = "User Name already exists!" });

                string passwordCheck = CheckPasswordStrength(userObj.Password);
                if (!string.IsNullOrEmpty(passwordCheck))
                    return BadRequest(new { Message = passwordCheck });

                User user = new User()
                {
                    Username = userObj.Username,
                    FirstName = userObj.FirstName,
                    LastName = userObj.LastName,
                    Email = userObj.Email
                };
                user.Password = HashPassword(user, userObj.Password);
                await userRepo.AddAsync(user);
                return Ok(new { Message = "User Registered!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

        // Call this during Registration
        private string HashPassword(User user, string password)
        {
            return hasher.HashPassword(user, password);
        }

        // Call this during Login
        private bool VerifyPassword(User user, string hashedPassword, string providedPassword)
        {
            var result = hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);

            return result == PasswordVerificationResult.Success;
        }

        private string CreateJwt(LoginRequest userObj)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(jwtSetting.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, userObj.Username)
                    }
                ),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder messageString = new StringBuilder();
            if (password.Length < 6)
                messageString.Append("Minimum password length should be 6." + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                messageString.Append("Password should be Alphanumeric." + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                messageString.Append("Password should contain special charcter" + Environment.NewLine);
            return messageString.ToString();

        }
    }
}
