using Microsoft.AspNetCore.Http;

namespace DataLayer.DTOs
{
    public class ImageUploadDTO
    {
        public string? Username { get; set; }
        public string? Comment { get; set; }
        public string? filename { get; set; }
        public IFormFile? File { get; set; }
    }
}
