using System;

namespace DataLayer.Models
{
    public class ImageRecord
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public string Username { get; set; }
        public string FileId { get; set; }
        public string ImageId { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Registration ? Registration {get; set;}
    }
}
