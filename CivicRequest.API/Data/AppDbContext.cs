using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CivicRequest.API.Models;

namespace CivicRequest.API.Data
{
    public class AppDbContext : IdentityDbContext<Officer>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Infrastrukturë", Description = "Rrugë, ujë, kanalizime" },
                new Category { Id = 2, Name = "Arsim", Description = "Shkolla, çerdhe" },
                new Category { Id = 3, Name = "Shëndetësi", Description = "Spitale, qendra shëndetësore" },
                new Category { Id = 4, Name = "Mjedis", Description = "Pastërti, parqe" },
                new Category { Id = 5, Name = "Të tjera", Description = "Kërkesa të ndryshme" }
            );
        }
    }
}