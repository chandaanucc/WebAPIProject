namespace villa.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
       
    }
}