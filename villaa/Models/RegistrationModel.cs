namespace villa.Models
{
    public class RegistrationModel
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required int IsActive { get; set; }
    }
}