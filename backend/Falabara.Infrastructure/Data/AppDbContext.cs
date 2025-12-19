using Microsoft.EntityFrameworkCore;
using Falabara.Domain.Entities;

namespace Falabara.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Complaints)
                    .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Complaint)
                    .WithMany(c => c.Comments)
                    .HasForeignKey(e => e.ComplaintId);
            });
        }
    }
}