using Microsoft.EntityFrameworkCore;
using DataLayer.Models;

namespace DataLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<MediaUpload> MediaUploads { get; set; }
        public DbSet<ImageRecord> FileRecords { get; set; }
        // public DbSet<ImageRecord> ImageRecords { get; set; }
        public DbSet<Registration> RegisteredUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your model here
            // modelBuilder.Entity<MediaUpload>()
            //     .HasOne(m => m.Registration)
            //     .WithMany(u=>u.Register)
            //     .HasForeignKey(m => m.RegistrationId)
            //     .OnDelete(DeleteBehavior.Cascade);
            // modelBuilder.Entity<MediaUpload>()
            //     .HasKey(m => m.Id);
            modelBuilder.Entity<MediaUpload>()
            .HasOne(m => m.Registration)
            .WithMany(r => r.MediaUpload)
            .HasForeignKey(m => m.RegistrationId);

            modelBuilder.Entity<ImageRecord>()
            .HasOne(c => c.Registration)
            .WithMany(q => q.Filerecord)
            .HasForeignKey(m => m.RegistrationId);

            
        }


    }
}