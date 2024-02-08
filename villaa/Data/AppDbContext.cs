// DataContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using villa.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RegistrationModel>RegisteredUsers { get; set; }
    public DbSet<LoginModel>LoginUsers { get; set; }
    public DbSet<ImageModel> ImageUploads { get; set; }
    public DbSet<VideoModel> VideoUploads { get; set; }
    public DbSet<CommentModel> Comments { get; set; }

}