// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Options;
// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.IdentityModel.Tokens;
// using RepositoryLayer.Interfaces;

// namespace villa.Helpers
// {
//     public class JwtMiddleware
//     {
//         private readonly RequestDelegate _next;
//         private readonly ILogger<JwtMiddleware> _logger;
//         private readonly AppSettings _appSettings;

//         public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, ILogger<JwtMiddleware> logger)
//         {
//             _next = next;
//             _logger = logger;
//             _appSettings = appSettings.Value;
//         }

//         public async Task Invoke(HttpContext context, IUserRepository userRepository)
//         {
//             var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

//             if (token != null)
//                 AttachUserToContext(context, userRepository, token);

//             await _next(context);
//         }

//         private void AttachUserToContext(HttpContext context, IUserRepository userRepository, string token)
//         {
//             try
//             {
//                 var tokenHandler = new JwtSecurityTokenHandler();
//                 var key = Encoding.ASCII.GetBytes(_appSettings.Token);
//                 tokenHandler.ValidateToken(token, new TokenValidationParameters
//                 {
//                     ValidateIssuerSigningKey = true,
//                     IssuerSigningKey = new SymmetricSecurityKey(key),
//                     ValidateIssuer = false,
//                     ValidateAudience = false,
//                     ClockSkew = TimeSpan.Zero
//                 }, out SecurityToken validatedToken);

//                 var jwtToken = (JwtSecurityToken)validatedToken;
//                 var username = jwtToken.Claims.First(x => x.Type == "name").Value;

//                 // attach user to context on successful jwt validation
//                 context.Items["User"] = userRepository.GetUserByUsernameAsync(username);
//             }
//             catch (SecurityTokenValidationException ex)
//             {
//                 // Log the validation exception
//                 _logger.LogError("JWT token validation failed: {0}", ex.Message);
//                 // You can add further error handling or response logic here if needed
//             }
//             catch (Exception ex)
//             {
//                 // Log any other exceptions
//                 _logger.LogError("An error occurred while processing JWT token: {0}", ex.Message);
//                 // You can add further error handling or response logic here if needed
//             }
//         }
//     }
// }
