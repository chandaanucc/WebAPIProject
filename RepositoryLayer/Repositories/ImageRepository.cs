using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Data;
using RepositoryLayer.Interfaces;
using DataLayer.Models;
using Microsoft.Extensions.Logging;

namespace RepositoryLayer.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(AppDbContext context, ILogger<ImageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddImageAsync(MediaUpload image, string username, string commentText)
        {
            try
            {
                var user = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                image.UserName = username;
                image.RegistrationId = user.RegistrationId;
                image.CommentText = commentText;
                _context.MediaUploads.Add(image);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Image added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding image");
                throw; // Rethrow the exception to propagate it further
            }
        }

        public async Task<MediaUpload> GetImageByUsernameAsync(string username)
        {
            try
            {
                return await _context.MediaUploads.FirstOrDefaultAsync(i => i.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image by username");
                throw; // Rethrow the exception to propagate it further
            }
        }

        public async Task EditImageAsync(MediaUpload image, string username, string commentText)
        {
            try
            {
                var existingImage = await _context.MediaUploads.FirstOrDefaultAsync(i => i.UserName == username);
                if (existingImage == null)
                {
                    throw new InvalidOperationException("Image not found");
                }

                // Update image properties
                existingImage.ImageName = image.ImageName;
                existingImage.ImageContent = image.ImageContent;
                existingImage.CommentText = commentText;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Image edited successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing image");
                throw; // Rethrow the exception to propagate it further
            }
        }

        public async Task DeleteImageAsync(string username)
        {
            try
            {
                var image = await _context.MediaUploads.FirstOrDefaultAsync(i => i.UserName == username);
                if (image != null)
                {
                    _context.MediaUploads.Remove(image);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Image deleted successfully");
                }
                else
                {
                    throw new InvalidOperationException("Image not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image");
                throw; // Rethrow the exception to propagate it further
            }
        }
    }
}
