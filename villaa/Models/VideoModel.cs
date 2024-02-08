// VideoModel.cs
namespace villa.Models
{
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class VideoModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VideoId { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string VideoName { get; set; }

    [Required]
    public required byte[] VideoContent { get; set; }
}
}
