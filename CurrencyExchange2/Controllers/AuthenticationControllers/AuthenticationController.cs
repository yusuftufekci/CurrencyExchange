using CurrencyExchange2.Model.Authentication;
using CurrencyExchange2.Requests;
using CurrencyExchange2.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyExchange2.Controllers.AuthenticationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public static User user = new User();
        public static PasswordInfo userPassword = new PasswordInfo();
        public static UserToken UserToken = new UserToken();

        public readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthenticationController( ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            return Ok(await _context.Users.ToListAsync());

        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<List<User>>> Register([FromBody] UserDto userDto)
        {
            var userExists = await _context.Users.SingleOrDefaultAsync(mytable => mytable.UserEmail == userDto.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode=400, Status = "Error", Message = "User already exists!" });
           
            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            user.UserEmail = userDto.Email;
            user.IpAdress = remoteIpAddress;
            userPassword.Password = passwordHash;
            userPassword.PasswordSalt = passwordSalt;
            userPassword.UserEmail = userDto.Email;
            _context.Users.Add(user);
            _context.PasswordInfos.Add(userPassword);
            await _context.SaveChangesAsync();
            return Ok(new Response {StatusCode=200, Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<List<User>>> Login([FromBody] UserDto userDto)
        {
            var user_param = await _context.PasswordInfos.SingleOrDefaultAsync(p => p.UserEmail == userDto.Email);
            if (user_param == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Username or password is incorrect" });
            if (VerifyPasswordHash(userDto.Password, user_param.Password, user_param.PasswordSalt))
            {
                var loginUser = await _context.Users.SingleOrDefaultAsync(p => p.UserEmail == userDto.Email);
                string token = CreateToken(loginUser);
                var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();


                var controlToken = await _context.UserTokens.SingleOrDefaultAsync(p => p.UserId == loginUser.UserId);
                loginUser.IpAdress = remoteIpAddress;
                loginUser.ModifiedDate = DateTime.UtcNow;
                if (controlToken == null)
                {
                    UserToken.Token = token;
                    UserToken.UserId = loginUser.UserId;
                    UserToken.ExpDate = new JwtSecurityTokenHandler().ReadToken(token).ValidTo;
                    _context.UserTokens.Add(UserToken);
                }
                else
                {
                    controlToken.Token = token;
                    controlToken.ExpDate = new JwtSecurityTokenHandler().ReadToken(token).ValidTo;
                    controlToken.ModifiedDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
                return Ok(new LoginResponse {StatusCode=200, Status = "Success", Message = "Login Succesfull", Token = token });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = 400, Status = "Error", Message = "Username or password is incorrect" });
            }
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserEmail)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
           
            return jwt;
        }


    }
}
