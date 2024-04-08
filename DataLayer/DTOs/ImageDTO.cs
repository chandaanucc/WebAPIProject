using Microsoft.AspNetCore.Http;

namespace DataLayer.DTOs
{
public class ImageDTO
{
    public string UserName { get; set; }
    public string Comment { get; set; }
    public string ImageName { get; set; }
    public IFormFile ImageContent { get; set; }
    //public string FileId { get; set; }
}
}