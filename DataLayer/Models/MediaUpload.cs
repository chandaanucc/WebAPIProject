using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class MediaUpload
    {
        
        public int Id { get; set; }
        
        public string ? UserName { get; set; }
        // public int ? ImageId { get; set; }
        public string ? ImageName { get; set; }
        public byte[] ? ImageContent { get; set; }
        // public int ? CommentId { get; set; }
        public string ? CommentText { get; set; }
        // public DateTime ? CommentTime { get; set; }
        // public int ? VideoId { get; set; }
        public string ? VideoName { get; set; }
        public byte[] ? VideoContent { get; set; }
        public string ? FileId { get; set; }
        public string ? MediaId { get; set; }
        public DateTime ? UploadedAt { get; set; }


        // Foreign key property
        public int RegistrationId { get; set;}

        // Navigation property
        
        public Registration ? Registration {get; set;}
    }
}
