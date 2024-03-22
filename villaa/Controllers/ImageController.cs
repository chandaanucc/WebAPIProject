using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using DataLayer.Models;
using RepositoryLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace villa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageRepository imageRepository, ILogger<ImageController> logger)
        {
            _imageRepository = imageRepository;
            _logger = logger;
        }

        
        [HttpPost("UploadImage/{username}")]
        public async Task<IActionResult> UploadImage(IFormFile file, string username, string commentText)
        {
            try
            {
                // if (file == null || file.Length == 0)
                // {
                //     _logger.LogWarning("File is empty");
                //     return BadRequest("File is empty");
                // }

                // Read the file content into a byte array
                byte[] imageData;
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    imageData = stream.ToArray();
                }

                // Create a MediaUpload object with the image data and username
                var image = new MediaUpload
                {
                    ImageName = file.FileName,
                    ImageContent = imageData
                };

                // Call the AddImageAsync method in the repository to save the image
                await _imageRepository.AddImageAsync(image, username, commentText);

                _logger.LogInformation("Image uploaded successfully");

                return Ok("Image uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
            }
        }

        [HttpGet("Get Image/{username}")]
        public async Task<IActionResult> GetImageByUserName(string username)
        {
            try
            {
                var image = await _imageRepository.GetImageByUsernameAsync(username);
                if (image == null)
                {
                    _logger.LogWarning($"Image not found for username: {username}");
                    return NotFound("Image not found");
                }

                _logger.LogInformation($"Image retrieved successfully for username: {username}");

                return File(image.ImageContent, "image/jpeg"); // Assuming JPEG images
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving image for username: {username}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving image: {ex.Message}");
            }
        }

        [HttpPut("edit/{username}")]
        public async Task<IActionResult> EditImageByUserName(IFormFile file, string username, string commentText)
        {
            try
            {
                if (file == null || file.Length <= 0)
                {
                    _logger.LogWarning("Invalid file");
                    return BadRequest("Invalid file");
                }

                // Retrieve existing image by username
                var existingImage = await _imageRepository.GetImageByUsernameAsync(username);
                if (existingImage == null)
                {
                    _logger.LogWarning($"Image not found for username: {username}");
                    return NotFound("Image not found");
                }

                // Update image content
                using (var stream = file.OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        existingImage.ImageName = file.FileName;
                        existingImage.ImageContent = memoryStream.ToArray();
                    }
                }

                // Call repository method to edit image
                await _imageRepository.EditImageAsync(existingImage, username, commentText);

                _logger.LogInformation($"Image updated successfully for username: {username}");

                return Ok("Image updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating image for username: {username}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating image: {ex.Message}");
            }
        }

        [HttpDelete("delete/{username}")]
        public async Task<IActionResult> DeleteImageByUserName(string username)
        {
            try
            {
                // Call repository method to delete image
                await _imageRepository.DeleteImageAsync(username);

                _logger.LogInformation($"Image deleted successfully for username: {username}");

                return Ok("Image deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image for username: {username}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting image: {ex.Message}");
            }
        }
    }
}
