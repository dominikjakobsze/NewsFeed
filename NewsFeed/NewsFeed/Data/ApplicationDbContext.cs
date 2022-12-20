using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsFeed.Models;

namespace NewsFeed.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<NewsFeed.Models.Category> Category { get; set; }
        public DbSet<NewsFeed.Models.News> News { get; set; }
        public DbSet<NewsFeed.Models.Comment> Comment { get; set; }
        public DbSet<NewsFeed.Models.AdminRole> AdminRole { get; set; }
    }
}