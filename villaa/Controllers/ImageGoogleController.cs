
using Microsoft.Extensions.Logging;
using InfrastructureLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InfrastructureLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageGoogleController : ControllerBase
    {
        private readonly IGoogleDriveService _googleDriveService;
        private readonly ILogger<ImageGoogleController> _logger;

        public ImageGoogleController(IGoogleDriveService googleDriveService, ILogger<ImageGoogleController> logger)
        {
            _googleDriveService = googleDriveService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, string filename, string username)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("No file uploaded.");
                    return BadRequest("No file uploaded.");
                }

                if (string.IsNullOrWhiteSpace(filename))
                {
                    _logger.LogWarning("Filename is required.");
                    return BadRequest("Filename is required.");
                }

                // Upload file to Google Drive
                var fileId = await _googleDriveService.UploadFileAsync(file, filename);
                _logger.LogInformation("File uploaded successfully. File ID: {FileId}", fileId);

                return Ok(new { FileId = fileId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadImage(string fileId)
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
                return File(fileBytes, "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading the file.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
