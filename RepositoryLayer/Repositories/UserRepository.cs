using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using DataLayer.Data;
using DataLayer.Models;
using RepositoryLayer.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace RepositoryLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IValidator<Registration> _registrationValidator;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, IValidator<Registration> registrationValidator, ILogger<UserRepository> logger)
        {
            _context = context;
            _registrationValidator = registrationValidator;
            _logger = logger;
        }

        public async Task<Registration> AuthenticateAsync(string username, string password)
        {
            // Implement authentication logic here
            // Example: Retrieve user from the database based on username and password
            var user = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
            return user;
        }
        public async Task CreateUserAsync(Registration user)
        {
            try
            {
                // Validate the user entity
                var validationResult = await _registrationValidator.ValidateAsync(user);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                _context.RegisteredUsers.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw; // Rethrow the exception to propagate it further
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                return await _context.RegisteredUsers.AnyAsync(u => u.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists");
                throw; // Rethrow the exception to propagate it further
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                return await _context.RegisteredUsers.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists");
                throw; // Rethrow the exception to propagate it further
            }
        }

        public async Task<Registration> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by username");
                throw; // Rethrow the exception to propagate it further
            }
        }
    }
}
