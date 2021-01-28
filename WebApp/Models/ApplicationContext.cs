using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

namespace network2.Models
{
    public class ApplicationContext : DbContext
    {
        
        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<PostModel> PostModels { get; set; }
        public DbSet<CommentModel> CommentModels { get; set; }
        public DbSet<PhotoModel> PhotoModels { get; set; }
        public DbSet<SongModel> SongModels { get; set; }
        public DbSet<NotificationModel> NotificationModels { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}