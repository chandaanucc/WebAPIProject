using System.ComponentModel.DataAnnotations;

namespace villa.Models
{
    public class CommentModel
    {
        
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string CommentText { get; set; }
        public DateTime Timestamp { get; set; }

    }
}