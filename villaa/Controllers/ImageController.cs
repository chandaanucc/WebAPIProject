using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using DataLayer.Models;
using RepositoryLayer.Interfaces;
using Microsoft.Extensions.Logging;
using DataLayer.Data;
using DataLayer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace villa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ImageController> _logger;
        private readonly AppDbContext _context;

        public ImageController(IImageRepository imageRepository, ILogger<ImageController> logger, AppDbContext context)
        {
            _imageRepository = imageRepository;
            _logger = logger;
            _context = context;
        }
        [HttpPost("uploadimage")]
        public async Task<IActionResult> UploadImage(
            [FromForm] ImageDTO imageDto
        )
        {
            if (imageDto.ImageContent == null || imageDto.ImageContent.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                // Check file extension
                string fileExtension = Path.GetExtension(imageDto.ImageContent.FileName).ToLowerInvariant();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type. Only images (.jpg, .jpeg, .png, .gif) are allowed.");
                }

                // Find the registered user by username
                var user = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == imageDto.UserName);
                if (user == null)
                {
                    return BadRequest("Invalid username. User not found.");
                }

                // Read the image data into a byte array
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    await imageDto.ImageContent.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                // Save the image data to the database
                var image = new MediaUpload
                {
                    UserName = imageDto.UserName,
                    ImageContent = imageData,
                    CommentText = imageDto.Comment,
                    RegistrationId = user.RegistrationId // Associate the image with the user's RegistrationId
                };

                await _imageRepository.AddAsync(image);

                return Ok("Image uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                // Log the error including inner exception details
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException.Message);
                }
                return StatusCode(500, "An error occurred while saving the image. See the logs for details.");
            }
        }
        // [HttpGet("GetAllImages")]
        // public async Task<ActionResult<IEnumerable<MediaUpload>>> GetAllImages()
        // {
        //     var images = await _imageRepository.GetAllAsync();
        //     return Ok(images);
        // }
        
        [HttpGet("ViewImage/{id}")]
        //[HttpGet("GetImage/{id}")]
public async Task<IActionResult> ViewImage(int id)
{
    // Fetch image data from the repository based on the provided ID
    var imageData = await _imageRepository.GetImageByIdAsync(id);

    if (imageData == null)
    {
        // If no image data is found for the given ID, return a not found response
        return NotFound();
    }

    // Return the image data as a file response
    return File(imageData.ImageContent, "image/jpeg"); // Adjust content type as needed
}
        // [HttpPut("UpdateImage/{id}")]
        // public async Task<ActionResult> UpdateImage(int id, [FromBody] ImageDTO imageDTO)
        // {
            
        //     var existingImage = await _imageRepository.GetByIdAsync(id);
        //     if (existingImage == null)
        //     {
        //         return NotFound();
        //     }
        //     byte[] imageData;
        //         using (var memoryStream = new MemoryStream())
        //         {
        //             await imageDTO.ImageContent.CopyToAsync(memoryStream);
        //             imageData = memoryStream.ToArray();
        //         }

        //     existingImage.UserName = imageDTO.UserName;
        //     existingImage.ImageName = imageDTO.ImageName;
        //     existingImage.ImageContent = imageData;
        //     existingImage.CommentText = imageDTO.Comment;
        //     existingImage.FileId = imageDTO.FileId;

        //     try
        //     {
        //         await _imageRepository.UpdateAsync(existingImage);
        //         return NoContent();
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"An error occurred: {ex.Message}");
        //     }
        // }

        [HttpDelete("DeleteImage/{id}")]
        public async Task<ActionResult> DeleteImage(int id)
        {
            var existingImage = await _imageRepository.GetByIdAsync(id);
            if (existingImage == null)
            {
                return NotFound();
            }

            try
            {
                await _imageRepository.DeleteAsync(existingImage);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
