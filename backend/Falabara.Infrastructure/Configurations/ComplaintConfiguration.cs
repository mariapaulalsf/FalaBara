using Falabara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falabara.Infrastructure.Configurations
{
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.ToTable("Complaints");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title).HasMaxLength(150).IsRequired();
            builder.Property(c => c.Description).HasMaxLength(2000).IsRequired();
            builder.Property(c => c.Location).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Neighborhood).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.Category).IsRequired();

            builder.HasOne(c => c.User)
                   .WithMany() 
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}