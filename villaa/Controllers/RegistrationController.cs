using Microsoft.AspNetCore.Mvc;
using DataLayer.Models;
using RepositoryLayer.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace villa.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(IUserRepository userRepository, ILogger<RegistrationController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Registration model)
        {
            // Validate user credentials
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid login request: {0}", ModelState);
                return BadRequest("Invalid login request");
            }

            // Authenticate user (check credentials against database, etc.)
            var user = await _userRepository.AuthenticateAsync(model.UserName, model.Password);
            if (user == null)
            {
                _logger.LogWarning("Invalid username or password");
                return Unauthorized("Invalid username or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            _logger.LogInformation("User authenticated successfully");
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(Registration user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
           var key = new byte[256 / 8]; // 128 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            var base64Key = Convert.ToBase64String(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> AddUser(Registration registration)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid registration data: {0}", ModelState);
                    return BadRequest(ModelState);
                }

                if (await _userRepository.UserExistsAsync(registration.UserName))
                {
                    _logger.LogWarning("User with the same UserName already registered");
                    return BadRequest("User with the same UserName already registered");
                }

                if (await _userRepository.EmailExistsAsync(registration.Email))
                {
                    _logger.LogWarning("User with the same Email already registered");
                    return BadRequest("User with the same Email already registered");
                }

                await _userRepository.CreateUserAsync(registration);

                _logger.LogInformation("User registration successful");

                return Ok("User registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return BadRequest($"Error occurred during user registration: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetUser/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    _logger.LogWarning("User not found for username: {0}", username);
                    return NotFound("User not found");
                }

                _logger.LogInformation("User retrieved successfully");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving user: {username}");
                return BadRequest($"Error occurred while retrieving user: {ex.Message}");
            }
        }
    }
}
