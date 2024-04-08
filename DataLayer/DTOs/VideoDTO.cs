// VideoDTO.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.DTOs
{
    public class VideoDTO
    {
        
        public string? Username { get; set; }
        public string? Comment { get; set; }
        public string? Videoname { get; set; }
        public IFormFile? File { get; set; }
        
    }
}
