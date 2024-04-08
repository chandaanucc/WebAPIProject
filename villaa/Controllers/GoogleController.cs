using Microsoft.Extensions.Logging;
using InfrastructureLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using DataLayer.DTOs;
using DataLayer.Data;
using DataLayer.Models;
using Microsoft.AspNetCore.Authorization;

namespace villa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageGoogleController : ControllerBase
    {
        private readonly IGoogleDriveService _googleDriveService;
        private readonly ILogger<ImageGoogleController> _logger;
        private readonly AppDbContext _dbContext; // Inject your DbContext

        public ImageGoogleController(IGoogleDriveService googleDriveService, ILogger<ImageGoogleController> logger, AppDbContext dbContext)
        {
            _googleDriveService = googleDriveService;
            _logger = logger;
            _dbContext = dbContext; // Initialize your DbContext
        }

        [HttpPost("upload")]
        
        public async Task<IActionResult> UploadImage([FromForm]ImageUploadDTO imageUploadDto)
        {
            try
            {
                if (imageUploadDto.File == null || imageUploadDto.File.Length == 0)
                {
                    _logger.LogWarning("No file uploaded.");
                    return BadRequest("No file uploaded.");
                }

                if (string.IsNullOrWhiteSpace(imageUploadDto.filename))
                {
                    _logger.LogWarning("Filename is required.");
                    return BadRequest("Filename is required.");
                }

                if (string.IsNullOrWhiteSpace(imageUploadDto.Username))
                {
                    _logger.LogWarning("Username is required.");
                    return BadRequest("Username is required.");
                }

                var user = await _dbContext.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == imageUploadDto.Username);
                if (user == null)
                {
                    _logger.LogWarning("Username is not valid.");
                    return BadRequest("Username is not valid.");
                }


                // Upload file to Google Drive
                var fileId = await _googleDriveService.UploadFileAsync(imageUploadDto.File, imageUploadDto.filename);
                _logger.LogInformation("File uploaded successfully to Google Drive. File ID: {FileId}", fileId);

                // Store fileId and username in the database
                _dbContext.MediaUploads.Add(new MediaUpload
                {
                    FileId = fileId,
                    FileName = imageUploadDto.filename,
                    UserName = imageUploadDto.Username,
                    RegistrationId = user.RegistrationId,
                    UploadedAt = DateTime.Now, // Optionally, you can add upload date time
                    CommentText = imageUploadDto.Comment
                });
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("File ID and Username stored in the database.");

                return Ok(new { FileId = fileId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("images")]
        
        public async Task<ActionResult<IEnumerable<object>>> GetImages()
        {
            try
            {
                var mediaUploads = await _dbContext.MediaUploads
                .Where(m => m.VideoName == null)
                    .Select(m => new 
                    {
                        m.Id,
                        m.FileId,
                        m.FileName,
                        Username = m.UserName, // Use a different local variable for UserName
                        m.CommentText
                    })
                    .ToListAsync();

                return Ok(mediaUploads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching images with comments.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("viewimages/{fileId}")]
        
        public async Task<IActionResult> GetImage(string fileId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileId))
                {
                    _logger.LogWarning("File ID is required.");
                    return BadRequest("File ID is required.");
                }

                // Download image from Google Drive
                var fileBytes = await _googleDriveService.DownloadFileAsync(fileId);

                // Check if file was found
                if (fileBytes == null)
                {
                    _logger.LogWarning("File not found.");
                    return NotFound("File not found.");
                }

                _logger.LogInformation("File downloaded successfully. File ID: {FileId}", fileId);

                // Return image file content
                return File(fileBytes, "image/png"); // Adjust content type based on your image format
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the image.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

[HttpDelete("delete/{id}")]

public async Task<IActionResult> DeleteImage(int id)
{
    try
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid request. Please provide a valid image ID.");
            return BadRequest("Invalid request. Please provide a valid image ID.");
        }

        // Fetch corresponding entry from the database
        var mediaUpload = await _dbContext.MediaUploads.FindAsync(id);
        if (mediaUpload == null)
        {
            _logger.LogWarning("Image with ID {Id} not found.", id);
            return NotFound("Image not found.");
        }

        // Delete the entry from the database
        _dbContext.MediaUploads.Remove(mediaUpload);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Image deleted successfully. ID: {Id}", id);

        return StatusCode(200);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while deleting the image.");
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
}
[HttpPut("update/{fileId}")]

public async Task<IActionResult> UpdateImage(string fileId, [FromBody] UpdateImageDTO updateImageDto)
{
    try
    {
        if (string.IsNullOrWhiteSpace(fileId))
        {
            _logger.LogWarning("File ID is required.");
            return BadRequest("File ID is required.");
        }

        // Fetch corresponding entry from the database using fileId
        var mediaUpload = await _dbContext.MediaUploads.FirstOrDefaultAsync(u => u.FileId == fileId);
        if (mediaUpload == null)
        {
            _logger.LogWarning("Image with File ID {FileId} not found.", fileId);
            return NotFound("Image not found.");
        }

        // Update the comment
        mediaUpload.CommentText = updateImageDto.Comment;
        _dbContext.MediaUploads.Update(mediaUpload);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Image updated successfully. File ID: {FileId}", fileId);

        return StatusCode(200);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while updating the image.");
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
}
        [HttpGet("download/{fileId}")]
        
        public async Task<IActionResult> DownloadImage(string fileId)
        {
            {
            try
            {
                if (string.IsNullOrWhiteSpace(fileId))
                {
                    _logger.LogWarning("File ID is required.");
                    return BadRequest("File ID is required.");
                }

                // Download file from Google Drive
                var fileBytes = await _googleDriveService.DownloadFileAsync(fileId);

                // Check if file was found
                if (fileBytes == null)
                {
                    _logger.LogWarning("File not found.");
                    return NotFound("File not found.");
                }

                _logger.LogInformation("File downloaded successfully. File ID: {FileId}", fileId);

                // Return file content
                return File(fileBytes, "application/octet-stream","image.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading the file.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
            
        }
    }
}
