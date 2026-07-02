using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Common;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(TableNames.User);

            /*__IDUser__*/

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasConversion(
                    id => id.Value,
                    dbValue => UserId.From(dbValue))
                .HasColumnName("id")
                .ValueGeneratedNever();

            /*__Login__*/

            builder.Property(u => u.Login)
                .HasConversion(
                    login => login.Value,
                    dbValue => Login.Create(dbValue))
                .HasColumnName("login")
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
                .HasColumnName("user_name")
                .IsRequired();

            /*__Email__*/

            builder.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    dbValue => Email.Create(dbValue))
                .HasColumnName("email")
                .UseCollation(PostgresConstants.COLLATION_NAME)
                .IsRequired();

            builder.HasIndex(u => u.Email, "IX_Users_Email").IsUnique();

            builder.Property(a => a.AvatarKey)
                .HasColumnName("avatar_key")
                .HasConversion(key => key.HasValue ? key.Value.Value : null, db => !string.IsNullOrWhiteSpace(db) ? S3Key.Restore(db) : (S3Key?)null )
                .IsRequired(false);

            /*__Dates__*/

            builder.Property(u => u.DateRegistration)
                .HasColumnName("date_registration").IsRequired();

            builder.Property(u => u.DateUpdate)
                .HasColumnName("date_update").IsRequired();

            builder.Property(u => u.DateEntry)
                .HasColumnName("date_entry").IsRequired(false);

            /*__Ids__*/

            builder.Property(u => u.IdStatus)
                .HasColumnName("id_status")
                .IsRequired();

            builder.Property(u => u.IdGender)
                .HasColumnName("id_gender")
                .IsRequired(false);

            builder.Property(u => u.IdCountry)
                .HasColumnName("id_country")
                .IsRequired(false);

            /*__Связи__*/

            builder.HasMany(u => u.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(u => u.UserRoles).HasField("_userRoles").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(u => u.Deks)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(u => u.Deks).HasField("_deks").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(u => u.UserAuthenticators)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(u => u.UserAuthenticators).HasField("_userAuthenticators").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(u => u.RecoveryKeys)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(u => u.RecoveryKeys).HasField("_recoveryKeys").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore("_domainEvents");
        }
    }
}