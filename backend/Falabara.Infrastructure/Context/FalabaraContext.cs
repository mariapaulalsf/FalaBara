using Falabara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Falabara.Infrastructure.Context
{
    public class FalabaraContext : DbContext
    {
        public FalabaraContext(DbContextOptions<FalabaraContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Complaint> Complaints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}