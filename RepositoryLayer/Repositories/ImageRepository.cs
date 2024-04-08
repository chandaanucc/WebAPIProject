using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLayer.Data;
using RepositoryLayer.Interfaces;
using DataLayer.Models;
using Microsoft.Extensions.Logging;

namespace RepositoryLayer.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(AppDbContext context, ILogger<ImageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

         public async Task AddAsync(MediaUpload image)
        {
            try
            {
                _context.MediaUploads.Add(image);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Image added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding image");
                throw; // Rethrow the exception to propagate it further
            }
        }
        public async Task<MediaUpload> GetByIdAsync(int Id)
        {
            return await _context.MediaUploads.FindAsync(Id);
        }

        public async Task<IEnumerable<MediaUpload>> GetAllAsync()
        {
            return await _context.MediaUploads.ToListAsync();
        }
         public async Task<MediaUpload> GetImageByIdAsync(int id)
        {
            return await _context.MediaUploads.FirstOrDefaultAsync(image => image.Id == id);
        }
         public async Task UpdateAsync(MediaUpload image)
        {
            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MediaUpload image)
        {
            _context.MediaUploads.Remove(image);
            await _context.SaveChangesAsync();
        }

        
    }
}
