// // VideoController.cs
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using DataLayer.DTOs;
// using DataLayer.Models;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using RepositoryLayer.Interfaces;
// using DataLayer.Data;

// namespace villa.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class VideoController : ControllerBase
//     {
//         private readonly IVideoRepository _videoRepository;
//         private readonly ILogger<VideoController> _logger;
//         private readonly AppDbContext _context;

//         public VideoController(IVideoRepository videoRepository, ILogger<VideoController> logger, AppDbContext context)
//         {
//             _videoRepository = videoRepository;
//             _logger = logger;
//             _context = context;
//         }

//         [HttpPost("uploadvideo")]
//         public async Task<IActionResult> UploadVideo([FromForm] VideoDTO videoDto)
//         {
//             if (videoDto.VideoContent == null || videoDto.VideoContent.Length == 0)
//                 return BadRequest("No file uploaded.");

//             try
//             {
//                 // Check file extension
//                 string fileExtension = Path.GetExtension(videoDto.VideoContent.FileName).ToLowerInvariant();
//                 string[] allowedExtensions = { ".mp4", ".avi", ".mov", ".wmv" };

//                 if (!allowedExtensions.Contains(fileExtension))
//                 {
//                     return BadRequest("Invalid file type. Only videos (.mp4, .avi, .mov, .wmv) are allowed.");
//                 }

//                 // Find the registered user by username
//                 var user = await _context.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == videoDto.UserName);
//                 if (user == null)
//                 {
//                     return BadRequest("Invalid username. User not found.");
//                 }

//                 // Read the video data into a byte array
//                 byte[] videoData;
//                 using (var memoryStream = new MemoryStream())
//                 {
//                     await videoDto.VideoContent.CopyToAsync(memoryStream);
//                     videoData = memoryStream.ToArray();
//                 }

//                 // Save the video data to the database
//                 var video = new MediaUpload
//                 {
//                     UserName = videoDto.UserName,
//                     VideoContent = videoData,
//                     CommentText = videoDto.Comment,
//                     RegistrationId = user.RegistrationId // Associate the video with the user's RegistrationId
//                 };

//                 await _videoRepository.AddAsync(video);

//                 return Ok("Video uploaded successfully");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex.Message);
//                 // Log the error including inner exception details
//                 if (ex.InnerException != null)
//                 {
//                     _logger.LogError(ex.InnerException.Message);
//                 }
//                 return StatusCode(500, "An error occurred while saving the video. See the logs for details.");
//             }
//         }

//         [HttpGet("ViewVideo/{id}")]
//         public async Task<IActionResult> ViewVideo(int id)
//         {
//             // Fetch video data from the repository based on the provided ID
//             var videoData = await _videoRepository.GetVideoByIdAsync(id);

//             if (videoData == null)
//             {
//                 // If no video data is found for the given ID, return a not found response
//                 return NotFound();
//             }

//             // Return the video data as a file response
//             return File(videoData.VideoContent, "video/mp4"); // Adjust content type as needed
//         }

//         [HttpDelete("DeleteVideo/{id}")]
//         public async Task<ActionResult> DeleteVideo(int id)
//         {
//             var existingVideo = await _videoRepository.GetByIdAsync(id);
//             if (existingVideo == null)
//             {
//                 return NotFound();
//             }

//             try
//             {
//                 await _videoRepository.DeleteAsync(existingVideo);
//                 return NoContent();
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"An error occurred: {ex.Message}");
//             }
//         }
//     }
// }
