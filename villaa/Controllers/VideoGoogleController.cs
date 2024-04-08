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
    public class VideoGoogleController : ControllerBase
    {
        private readonly IGoogleDriveService _googleDriveService;
        private readonly ILogger<VideoGoogleController> _logger;
        private readonly AppDbContext _dbContext; // Inject your DbContext

        public VideoGoogleController(IGoogleDriveService googleDriveService, ILogger<VideoGoogleController> logger, AppDbContext dbContext)
        {
            _googleDriveService = googleDriveService;
            _logger = logger;
            _dbContext = dbContext; // Initialize your DbContext
        }

        [HttpPost("upload")]
        
        public async Task<IActionResult> UploadVideo([FromForm]VideoDTO videoUploadDto)
        {
            try
            {
                if (videoUploadDto.File == null || videoUploadDto.File.Length == 0)
                {
                    _logger.LogWarning("No file uploaded.");
                    return BadRequest("No file uploaded.");
                }

                if (string.IsNullOrWhiteSpace(videoUploadDto.Videoname))
                {
                    _logger.LogWarning("Videoname is required.");
                    return BadRequest("Videoname is required.");
                }

                if (string.IsNullOrWhiteSpace(videoUploadDto.Username))
                {
                    _logger.LogWarning("Username is required.");
                    return BadRequest("Username is required.");
                }

                var user = await _dbContext.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == videoUploadDto.Username);
                if (user == null)
                {
                    _logger.LogWarning("Username is not valid.");
                    return BadRequest("Username is not valid.");
                }


                // Upload file to Google Drive
                var fileId = await _googleDriveService.UploadFileAsync(videoUploadDto.File, videoUploadDto.Videoname);
                _logger.LogInformation("Video File uploaded successfully to Google Drive. File ID: {FileId}", fileId);

                // Store fileId and username in the database
                _dbContext.MediaUploads.Add(new MediaUpload
                {
                    FileId = fileId,
                    VideoName = videoUploadDto.Videoname,
                    UserName = videoUploadDto.Username,
                    RegistrationId = user.RegistrationId,
                    UploadedAt = DateTime.Now, // Optionally, you can add upload date time
                    CommentText = videoUploadDto.Comment
                });
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Video File ID and Username stored in the database.");

                return Ok(new { FileId = fileId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("videos")]
        public async Task<ActionResult<IEnumerable<object>>> GetVideos()
        {
            try
            {
                var mediaUploads = await _dbContext.MediaUploads
                    .Where(m => m.FileName == null)
                    .Select(m => new 
                    {
                        m.Id,
                        m.FileId,
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

        [HttpGet("viewvideos/{fileId}")]
        
        public async Task<IActionResult> GetVideos(string fileId)
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
                return File(fileBytes, "video/mp4"); // Adjust content type based on your image format
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the image.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

[HttpDelete("delete/{id}")]

public async Task<IActionResult> DeleteVideo(int id)
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
            _logger.LogWarning("Video with ID {Id} not found.", id);
            return NotFound("Video not found.");
        }

        // Delete the entry from the database
        _dbContext.MediaUploads.Remove(mediaUpload);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Video deleted successfully. ID: {Id}", id);

        return StatusCode(200);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while deleting the video.");
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
}
[HttpPut("update/{fileId}")]

public async Task<IActionResult> UpdateImage(string fileId, [FromBody] UpdateImageDTO updateVideoDto)
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
            _logger.LogWarning("Video with File ID {FileId} not found.", fileId);
            return NotFound("Video not found.");
        }

        // Update the comment
        mediaUpload.CommentText = updateVideoDto.Comment;
        _dbContext.MediaUploads.Update(mediaUpload);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Video updated successfully. File ID: {FileId}", fileId);

        return StatusCode(200);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while updating the image.");
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
}
        [HttpGet("download/{fileId}")]
        
        public async Task<IActionResult> DownloadVideo(string fileId)
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
                return File(fileBytes, "application/octet-stream","video.mp4");
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
