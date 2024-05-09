using Domain.Bislerium.Enums;
using Domain.Bislerium.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium
{
    public class ApplicationDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=RONIM\\SQLEXPRESS;Database=Bislerium;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete on User deletion

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reactions)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            SeedUsers(modelBuilder);
        }


        // Seed Admin at first
        private void SeedUsers(ModelBuilder builder)
        {
            User user = new User
            {
                ID=Guid.NewGuid(),
                Role = UserRole.ADMIN,
                UserName = "Admin Ripesh",
                Email = "admin@gmail.com",
            };
            user.Password = BCrypt.Net.BCrypt.HashPassword("admin");
            builder.Entity<User>().HasData(user);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<FirebaseToken> FirebaseTokens { get; set; }
    }
}