using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Write
{
    internal sealed class UserWriteConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            /*__IDUser__*/

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasConversion(
                    id => id.Value,
                    dbValue => UserId.From(dbValue))
                .HasColumnName("Id")
                .ValueGeneratedNever();

            /*__Login__*/

            builder.Property(u => u.Login)
                .HasConversion(
                    login => login.Value,
                    dbValue => Login.Create(dbValue))
                .HasColumnName("Login")
                .HasMaxLength(Login.MAX_LENGTH)
                .UseCollation(PostgresConstants.COLLATION_NAME)
                .IsRequired();

            builder.HasIndex(u => u.Login, "IX_Users_Login").IsUnique();

            /*__UserName__*/

            builder.Property(u => u.UserName)
                .HasConversion(
                    userName => userName.Value,
                    dbValue => new UserName(dbValue))
                .HasMaxLength(UserName.MAX_LENGTH)
                .UseCollation(PostgresConstants.COLLATION_NAME)
                .HasColumnName("UserName")
                .IsRequired();

            /*__PasswordHash__*/

            builder.Property(u => u.PasswordHash)
                .HasConversion(
                    passHash => passHash.Value,
                    dbValue => PasswordHash.Create(dbValue))
                .HasColumnName("PasswordHash")
                .IsRequired();

            /*__Email__*/

            builder.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    dbValue => Email.Create(dbValue))
                .HasColumnName("Email")
                .UseCollation(PostgresConstants.COLLATION_NAME)
                .IsRequired();

            builder.HasIndex(u => u.Email, "IX_Users_Email").IsUnique();

            /*__Phone__*/

            builder.Property(u => u.Phone)
                .HasConversion(
                    phone => phone != null ? phone.Value : null,
                    dbValue => !string.IsNullOrEmpty(dbValue) ? new Phone(dbValue) : null)
                .HasMaxLength(20)
                .HasColumnName("Phone")
                .IsRequired(false);

            builder.HasIndex(u => u.Phone, "IX_Users_Phone")
                .IsUnique()
                .HasFilter("\"Phone\" IS NOT NULL");

            /*__Dates__*/

            builder.Property(u => u.DateRegistration)
                .HasColumnName("DateRegistration").IsRequired();

            builder.Property(u => u.DateUpdate)
                .HasColumnName("DateUpdate").IsRequired();

            builder.Property(u => u.DateEntry)
                .HasColumnName("DateEntry").IsRequired(false);

            /*__Ids__*/

            builder.Property(u => u.IdStatus)
                .HasColumnName("IdStatus")
                .IsRequired();

            builder.Property(u => u.IdRole)
                .HasColumnName("IdRole")
                .IsRequired();

            builder.Property(u => u.IdGender)
                .HasColumnName("IdGender")
                .IsRequired(false);

            builder.Property(u => u.IdCountry)
                .HasColumnName("IdCountry")
                .IsRequired(false);

            /*__Связи__*/

            builder.HasOne(u => u.Gender)
                .WithMany()
                .HasForeignKey(u => u.IdGender)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey(u => u.IdCountry)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.IdRole)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Status)
                .WithMany()
                .HasForeignKey(u => u.IdStatus)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore("_domainEvents");
        }
    }
}