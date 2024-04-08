// VideoRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Data;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VideoRepository> _logger;

        public VideoRepository(AppDbContext context, ILogger<VideoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(MediaUpload video)
        {
            try
            {
                _context.MediaUploads.Add(video);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Video added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding video");
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

        public async Task<MediaUpload> GetVideoByIdAsync(int id)
        {
            return await _context.MediaUploads.FirstOrDefaultAsync(video => video.Id == id);
        }

        public async Task UpdateAsync(MediaUpload video)
        {
            _context.Entry(video).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MediaUpload video)
        {
            _context.MediaUploads.Remove(video);
            await _context.SaveChangesAsync();
        }
    }
}
