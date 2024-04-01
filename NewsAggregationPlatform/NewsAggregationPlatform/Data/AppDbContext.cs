using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UserPositivityLevel> UserPositivityLevels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //cascade delete for Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Article)
                .WithMany(a => a.Comments)
                .OnDelete(DeleteBehavior.Cascade);
            //clientSetNull delete for Comments
            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.User)
            //    .WithMany(u => u.Comments)
            //    .OnDelete(DeleteBehavior.ClientSetNull);

            //cascade delete for Ratings
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Article)
                .WithMany(a => a.Ratings)
                .OnDelete(DeleteBehavior.Cascade);

            //cascade delete for UserPositivityLevels
            modelBuilder.Entity<UserPositivityLevel>()
                .HasOne(upl => upl.Article)
                .WithMany(a => a.PositivityLevels)
                .OnDelete(DeleteBehavior.Cascade);

            //set CreatedByUserId to null when the associated user is deleted
            modelBuilder.Entity<Article>()
                .HasOne(a => a.User)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            //cascade delete for user comments when the user is deleted
            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);

            //cascade delete for UserPositivityLevels when user is deleted
            modelBuilder.Entity<UserPositivityLevel>()
                .HasOne(upl => upl.User)
                .WithMany(u => u.UserPositivityLevels)
                .OnDelete(DeleteBehavior.Cascade);

            //cascade delete for Ratings when user is deleted
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
