using System.Threading.Tasks;
using DataLayer.Models;

namespace RepositoryLayer.Interfaces
{
    public interface IImageRepository
    {
        Task AddImageAsync(MediaUpload image, string username, string commentText);
        Task<MediaUpload> GetImageByUsernameAsync(string username);
        Task EditImageAsync(MediaUpload image, string username, string commentText);
        Task DeleteImageAsync(string username);
        
    }

    
}
