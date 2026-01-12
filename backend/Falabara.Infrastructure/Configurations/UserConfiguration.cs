using Falabara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falabara.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("id");

            builder.Property(u => u.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();
            
            // não permitir emails duplicados no banco
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(14) 
                .IsRequired();

            //  regra de "1 conta por CPF"
            builder.HasIndex(u => u.Cpf).IsUnique();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            builder.Property(u => u.Type)
                .HasColumnName("user_type")
                .IsRequired();
            
            builder.Property(u => u.Active)
                .HasColumnName("active")
                .IsRequired();

            builder.Property(u => u.Department)
                .HasColumnName("department")
                .HasMaxLength(100);

            builder.Property(u => u.FoneNumber)
                .HasColumnName("fone_number")
                .HasMaxLength(20);
        }
    }
}