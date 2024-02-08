namespace villa.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        // public required string Password { get; set; }
        public required string ImageName { get; set; }
        public required byte[] ImageContent { get; set; }
    

    }

}