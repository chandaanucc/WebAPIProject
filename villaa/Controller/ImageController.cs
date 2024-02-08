using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using villa.Models;

namespace villa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("UploadImage")]
        public IActionResult UploadImage( string username,  IFormFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username is required");
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file");
                }

                // Check if the username exists in the RegisteredUsers DbSet
                var user = _context.RegisteredUsers.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    return NotFound("Register First");
                }

                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);

                    var image = new ImageModel
                    {
                        ImageName = file.FileName,
                        Username = username,
                        ImageContent = memoryStream.ToArray()
                    };

                    _context.ImageUploads.Add(image);
                    _context.SaveChanges();
                }

                return Ok("Image uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("ViewImage")]
        public IActionResult GetImage(string username)
        {
           try
    {
        var image = _context.ImageUploads.FirstOrDefault(i => i.Username == username);

        if (image == null)
        {
            return NotFound();
        }

        return File(image.ImageContent, "image/jpeg");
    }
    catch (Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
    }
        }
    
    [HttpDelete]
    [Route("DeleteImage")]
    public IActionResult DeleteImage(string username)
        {
            try
                {
                    var image = _context.ImageUploads.FirstOrDefault(i => i.Username == username);

                        if (image == null)
                            {
                                return NotFound($"Image for username '{username}' not found.");
                            }

                    _context.ImageUploads.Remove(image);
                    _context.SaveChanges();

                    return Ok($"Image for username '{username}' deleted successfully.");
                 }
            catch (Exception ex)
                {
                     return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
                }
        }   

        
    }
}
