// VideoController.cs

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using villa.Models;

namespace villa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VideoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("UploadVideo")]
        public IActionResult UploadVideo(IFormFile file, string username)
        {
            // try
            // {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file");
                }

                using (var memoryStream = new MemoryStream())
        {
            file.CopyTo(memoryStream);

            var video = new VideoModel
            {
                VideoName = file.FileName,
                Username = username,
                VideoContent = memoryStream.ToArray()
            };

                _context.VideoUploads.Add(video);
                _context.SaveChanges();

                return Ok("Video uploaded successfully");
            }
            }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        //     }
        // }

        [HttpGet]
        [Route("ViewVideo")]
        public IActionResult GetVideo(string username)
        {
            var video = _context.VideoUploads.FirstOrDefault(v => v.Username == username);

            if (video == null)
            {
                return NotFound($"Video for username '{username}' not found.");
            }

            return File(video.VideoContent, "video/mp4");
        }

        [HttpDelete]
        [Route("DeleteVideo")]
        public IActionResult DeleteVideo(string username)
        {
            var video = _context.VideoUploads.FirstOrDefault(v => v.Username == username);

            if (video == null)
            {
                return NotFound($"Video for username '{username}' not found.");
            }

            _context.VideoUploads.Remove(video);
            _context.SaveChanges();

            return Ok($"Video for username '{username}' deleted successfully");
        }
    }
}
