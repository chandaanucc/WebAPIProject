// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;

// namespace villa.Controller
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class RegistrationController : ControllerBase
//     {
//         // Simulating registered users with username and password stored in a dictionary
//         private static Dictionary<string, string> registeredUsers = new Dictionary<string, string>();

//         [HttpPost]
//         [Route("register")]
//         public IActionResult Register(string username, string password)
//         {
//             try
//             {
//                 // Validate inputs
//                 if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
//                 {
//                     return BadRequest("Username and password are required for registration.");
//                 }

//                 // Check if the username already exists
//                 if (registeredUsers.ContainsKey(username))
//                 {
//                     return BadRequest("Username already exists. Please choose a different username.");
//                 }

//                 // In a real-world scenario, you would hash and salt the password
//                 // For simplicity, let's assume the password is stored as is (not recommended in production)

//                 // Register the username and password
//                 registeredUsers.Add(username, password);

//                 // You may want to return a more meaningful response
//                 return Ok("Registration successful");
//             }
//             catch (Exception ex)
//             {
//                 // Log the exception for debugging purposes
//                 Console.WriteLine($"Error during registration: {ex.Message}");
//                 return StatusCode(500, "Internal Server Error");
//             }
//         }

//         [HttpPost]
//         [Route("login")]
//         public IActionResult Login(string username, string password)
//         {
//             try
//             {
//                 // Validate inputs
//                 if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
//                 {
//                     return BadRequest("Username and password are required for login.");
//                 }

//                 // Check if the username exists
//                 if (registeredUsers.TryGetValue(username, out var storedPassword))
//                 {

//                     // Check if the provided password matches the stored password
//                     if (password == storedPassword)
//                     {
//                         return Ok("Login successful");
//                     }
//                 }

//                 return BadRequest("Invalid credentials");
//             }
//             catch (Exception ex)
//             {
//                 // Log the exception for debugging purposes
//                 Console.WriteLine($"Error during login: {ex.Message}");
//                 return StatusCode(500, "Internal Server Error");
//             }
//         }
//     }
// }


using villa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace villa.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(RegistrationModel registration)
        {
            // Validate the incoming registration data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the user with the same UserName or Email already exists
            if (UserExists(registration.Username) || EmailExists(registration.Email))
            {
                return BadRequest("User with the same UserName or Email already registered");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "INSERT INTO RegisteredUsers (UserName, Password, Email, IsActive) VALUES (@UserName, @Password, @Email, @IsActive)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", registration.Username);
                    cmd.Parameters.AddWithValue("@Password", registration.Password);
                    cmd.Parameters.AddWithValue("@Email", registration.Email);
                    cmd.Parameters.AddWithValue("@IsActive", registration.IsActive);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Registration Successful");
                    }
                    else
                    {
                        return BadRequest("Registration Failed");
                    }
                }
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginModel login)
        {
            // Validate the incoming login data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "SELECT UserName FROM RegisteredUsers WHERE UserName = @Username AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", login.Username);
                    cmd.Parameters.AddWithValue("@Password", login.Password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        return Ok("Login Successful");
                    }
                    else
                    {
                        return Unauthorized("Invalid Login Credentials");
                    }
                }
            }
        }

        private bool UserExists(string username)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM RegisteredUsers WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        private bool EmailExists(string email)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM RegisteredUsers WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
        }

       
    }
}