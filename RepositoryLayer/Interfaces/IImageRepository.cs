using System.Threading.Tasks;
using DataLayer.Models;

namespace RepositoryLayer.Interfaces
{
    public interface IImageRepository
    {
        // Task AddImageAsync(MediaUpload image, string username, string commentText);
        // Task<MediaUpload> GetImageByUsernameAsync(string username);
        // Task EditImageAsync(MediaUpload image, string username, string commentText);
        // Task DeleteImageAsync(string username);
        Task AddAsync(MediaUpload image);
        Task<MediaUpload> GetByIdAsync(int Id);
        Task<IEnumerable<MediaUpload>> GetAllAsync();
         Task UpdateAsync(MediaUpload image);
        Task DeleteAsync(MediaUpload image);
         Task<MediaUpload> GetImageByIdAsync(int id);
        
    }

    
}
