using System.ComponentModel.DataAnnotations;

namespace DataLayer.DTOs 
{
    public class LoginDTO
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}

}