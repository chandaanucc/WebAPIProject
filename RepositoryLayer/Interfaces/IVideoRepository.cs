using DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IVideoRepository
    {
        Task AddAsync(MediaUpload video);
        Task<MediaUpload> GetByIdAsync(int Id);
        Task<IEnumerable<MediaUpload>> GetAllAsync();
        Task<MediaUpload> GetVideoByIdAsync(int id);
        Task UpdateAsync(MediaUpload video);
        Task DeleteAsync(MediaUpload video);
    }
}
