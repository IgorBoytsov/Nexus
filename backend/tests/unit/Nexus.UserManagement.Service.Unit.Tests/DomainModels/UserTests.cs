using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Events;
using Nexus.UserManagement.Service.Domain.Exceptions;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Common;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Xunit;

namespace Nexus.UserManagement.Service.Unit.Tests.DomainModels
{
    public class UserTests
    {
        private static User CreateUser()
        {
            Login login = Login.Create("ValidLogin");
            UserName userName = UserName.Create("Valid User Name");
            Email email = Email.Create("valid-email@email.com");
            Guid statusId = Guid.NewGuid();
            Guid genderId = Guid.NewGuid();
            Guid countryId = Guid.NewGuid();

            User user = User.Create(login, userName, email, statusId, genderId, countryId);

            return user;
        }

        private static User CreateUserWithSrpAndMainDek()
        {
            User user = CreateUser();

            user.AddSrpAuthenticator(
                user.Login,
                Verificator.Create("verificator"),
                Salt.Create("salt"),
                SrpVersion.Create(1),
                CredentialBlob.Create("blob"),
                CryptoVersion.Create(1),
                AsymmetricKeyId.Create("asymmetric-key-id"),
                CryptoVersion.Create(1));

            user.AddMainDek(
                EncryptedValue.Create("encrypted"),
                Salt.Create("dekSalt"),
                CryptoVersion.Create(1));

            return user;
        }

        #region Create - Create

        [Fact]
        public void User_Create_ValidValues_ReturnUser()
        {
            Login login = Login.Create("ValidLogin");
            UserName userName = UserName.Create("Valid User Name");
            Email email = Email.Create("valid-email@email.com");
            Guid statusId = Guid.NewGuid();
            Guid genderId = Guid.NewGuid();
            Guid countryId = Guid.NewGuid();

            User user = User.Create(login, userName, email, statusId, genderId, countryId);

            Assert.Equal(login, user.Login);
            Assert.Equal(userName, user.UserName);
            Assert.Equal(email, user.Email);
            Assert.Equal(statusId, user.IdStatus);
            Assert.Equal(genderId, user.IdGender);
            Assert.Equal(countryId, user.IdCountry);
        }

        [Fact]
        public void User_Create_ValidValuesWithOutNotRequiredFields_ReturnUser()
        {
            Login login = Login.Create("ValidLogin");
            UserName userName = UserName.Create("Valid User Name");
            Email email = Email.Create("valid-email@email.com");
            Guid statusId = Guid.NewGuid();

            User user = User.Create(login, userName, email, statusId, null, null);

            Assert.Equal(login, user.Login);
            Assert.Equal(userName, user.UserName);
            Assert.Equal(email, user.Email);
            Assert.Equal(statusId, user.IdStatus);
            Assert.Null(user.IdGender);
            Assert.Null(user.IdCountry);
        }

        #endregion

        #region Create - ChangeUserName

        [Fact]
        public void User_ChangeUserName_ValidValues_ChangesUserName()
        {
            User user = CreateUser();

            UserName oldUserName = user.UserName;
            UserName newUserName = UserName.Create("New User Name");

            user.ChangeUserName(newUserName, null);

            Assert.NotEqual(oldUserName, newUserName);
        }

        [Fact]
        public void User_ChangeUserName_IdenticalName_NoChangesUserName()
        {
            User user = CreateUser();

            UserName oldUserName = user.UserName;
            UserName newUserName = oldUserName;

            user.ChangeUserName(newUserName, null);

            Assert.Equal(oldUserName, newUserName);
        }

        #endregion

        #region Create - AddRoles

        [Fact]
        public void User_AddRoles_ValidRoles_ReturnUserWithRoles()
        {
            User user = CreateUser();

            List<Role> roles = [Role.Create("User"), Role.Create("Admin")];

            foreach (var role in roles)
            {
                user.AddRole(role.Id);
            }

            Assert.NotEmpty(user.UserRoles);
            Assert.Equal(2, user.UserRoles.Count);
        }

        [Fact]
        public void User_AddRoles_IncludedRoles_RoleDontAdded()
        {
            User user = CreateUser();

            Role role1 = Role.Create("User");
            Role role2 = Role.Create("Admin");
            
            user.AddRole(role1.Id);
            user.AddRole(role2.Id);
            user.AddRole(role2.Id);

            Assert.NotEmpty(user.UserRoles);
            Assert.Equal(2, user.UserRoles.Count);
        }

        #endregion

        #region Create - RemoveRoles

        [Fact]
        public void User_RemoveRoles_RemovedRoles()
        {
            User user = CreateUser();

            Role role1 = Role.Create("User");
            Role role2 = Role.Create("Admin");
            Role role3 = Role.Create("SuperAdmin");

            user.AddRole(role1.Id);
            user.AddRole(role2.Id);
            user.AddRole(role3.Id);

            Assert.Equal(3, user.UserRoles.Count);
            Assert.NotEmpty(user.UserRoles);

            user.RemoveRole(role3.Id);

            Assert.Equal(2, user.UserRoles.Count);
            Assert.NotEmpty(user.UserRoles);
        }

        [Fact]
        public void User_RemoveRoles_NotExistRole_DontChangeCountRoles()
        {
            User user = CreateUser();

            Role role1 = Role.Create("User");
            Role role2 = Role.Create("Admin");
            Role role3 = Role.Create("SuperAdmin");

            user.AddRole(role1.Id);
            user.AddRole(role2.Id);

            Assert.Equal(2, user.UserRoles.Count);
            Assert.NotEmpty(user.UserRoles);

            user.RemoveRole(role3.Id);

            Assert.Equal(2, user.UserRoles.Count);
            Assert.NotEmpty(user.UserRoles);
        }

        #endregion

        #region Create - ChangePassword 

        [Fact]
        public void User_ChangePassword_ValidValues_ChangesPassword()
        {
            User user = CreateUserWithSrpAndMainDek();

            user.ChangePassword(
                Verificator.Create("newVerificator"),
                Salt.Create("newSalt"),
                SrpVersion.Create(2),
                CredentialBlob.Create("newBlob"),
                CryptoVersion.Create(2),
                AsymmetricKeyId.Create("new-asymmetric-key-id"),
                EncryptedValue.Create("newEncrypted"),
                Salt.Create("newDekSalt"),
                CryptoVersion.Create(2),
                CryptoVersion.Create(2));

            Assert.NotNull(user);
        }

        [Fact]
        public void User_ChangePassword_WithoutSrpAuthenticator_ThrowsException()
        {
            User user = CreateUser();

            user.AddMainDek(
                EncryptedValue.Create("encrypted"),
                Salt.Create("dekSalt"),
                CryptoVersion.Create(1));

            Assert.Throws<UserAuthenticatorException>(() => user.ChangePassword(
                Verificator.Create("newVerificator"),
                Salt.Create("newSalt"),
                SrpVersion.Create(2),
                CredentialBlob.Create("newBlob"),
                CryptoVersion.Create(2),
                AsymmetricKeyId.Create("new-asymmetric-key-id"),
                EncryptedValue.Create("newEncrypted"),
                Salt.Create("newDekSalt"),
                CryptoVersion.Create(2),
                CryptoVersion.Create(2)));
        }

        [Fact]
        public void User_ChangePassword_WithoutMainDek_ThrowsException()
        {
            User user = CreateUser();

            user.AddSrpAuthenticator(
                user.Login,
                Verificator.Create("verificator"),
                Salt.Create("salt"),
                SrpVersion.Create(1),
                CredentialBlob.Create("blob"),
                CryptoVersion.Create(1),
                AsymmetricKeyId.Create("asymmetric-key-id"),
                CryptoVersion.Create(1));

            Assert.Throws<DekException>(() => user.ChangePassword(
                Verificator.Create("newVerificator"),
                Salt.Create("newSalt"),
                SrpVersion.Create(2),
                CredentialBlob.Create("newBlob"),
                CryptoVersion.Create(2),
                AsymmetricKeyId.Create("new-asymmetric-key-id"),
                EncryptedValue.Create("newEncrypted"),
                Salt.Create("newDekSalt"),
                CryptoVersion.Create(2),
                CryptoVersion.Create(2)));
        }

        #endregion

        #region Create - ResetPassword 

        [Fact]
        public void User_ResetPassword_ValidValues_ResetsPassword()
        {
            User user = CreateUserWithSrpAndMainDek();

            user.AddRecoveryKey(
                EncryptedValue.Create("recovery"),
                CryptoVersion.Create(1),
                KeyHint.Create("hint"));

            DateTime oldDateUpdate = user.DateUpdate;

            user.ResetPassword(
                Verificator.Create("newVerificator"),
                Salt.Create("newSalt"),
                SrpVersion.Create(2),
                CredentialBlob.Create("newBlob"),
                CryptoVersion.Create(2),
                AsymmetricKeyId.Create("new-asymmetric-key-id"),
                EncryptedValue.Create("newEncrypted"),
                Salt.Create("newDekSalt"),
                CryptoVersion.Create(2),
                CryptoVersion.Create(2));

            Assert.Empty(user.RecoveryKeys);
            Assert.True(user.DateUpdate > oldDateUpdate);
            Assert.Contains(user.DomainEvents, e => e is UserPasswordResetDomainEvent);
        }

        [Fact]
        public void User_ResetPassword_WithoutSrpAuthenticator_ThrowsException()
        {
            User user = CreateUser();

            user.AddMainDek(
                EncryptedValue.Create("encrypted"),
                Salt.Create("dekSalt"),
                CryptoVersion.Create(1));

            Assert.Throws<UserAuthenticatorException>(() => user.ResetPassword(
                Verificator.Create("newVerificator"),
                Salt.Create("newSalt"),
                SrpVersion.Create(2),
                CredentialBlob.Create("newBlob"),
                CryptoVersion.Create(2),
                AsymmetricKeyId.Create("new-asymmetric-key-id"),
                EncryptedValue.Create("newEncrypted"),
                Salt.Create("newDekSalt"),
                CryptoVersion.Create(2),
                CryptoVersion.Create(2)));
        }

        [Fact]
        public void User_ResetPassword_WithoutMainDek_ThrowsException()
        {
            User user = CreateUser();

            user.AddSrpAuthenticator(
                user.Login,
                Verificator.Create("verificator"),
                Salt.Create("salt"),
                SrpVersion.Create(1),
                CredentialBlob.Create("blob"),
                CryptoVersion.Create(1),
                AsymmetricKeyId.Create("asymmetric-key-id"),
                CryptoVersion.Create(1));

            Assert.Throws<DekException>(() => user.ResetPassword(
                Verificator.Create("newVerificator"),
                Salt.Create("newSalt"),
                SrpVersion.Create(2),
                CredentialBlob.Create("newBlob"),
                CryptoVersion.Create(2),
                AsymmetricKeyId.Create("new-asymmetric-key-id"),
                EncryptedValue.Create("newEncrypted"),
                Salt.Create("newDekSalt"),
                CryptoVersion.Create(2),
                CryptoVersion.Create(2)));
        }

        #endregion

        #region Create - AddSrpAuthenticator 

        [Fact]
        public void User_AddSrpAuthenticator_ValidValues_AddsAuthenticator()
        {
            User user = CreateUser();

            user.AddSrpAuthenticator(
                user.Login,
                Verificator.Create("verificator"),
                Salt.Create("salt"),
                SrpVersion.Create(1),
                CredentialBlob.Create("blob"),
                CryptoVersion.Create(1),
                AsymmetricKeyId.Create("asymmetric-key-id"),
                CryptoVersion.Create(1));

            Assert.Single(user.UserAuthenticators);
            Assert.Contains(user.UserAuthenticators, a => a.Method == UserAuthenticatorType.SRP);
        }

        [Fact]
        public void User_AddSrpAuthenticator_DuplicateMethod_ThrowsException()
        {
            User user = CreateUser();

            user.AddSrpAuthenticator(
                user.Login,
                Verificator.Create("verificator"),
                Salt.Create("salt"),
                SrpVersion.Create(1),
                CredentialBlob.Create("blob"),
                CryptoVersion.Create(1),
                AsymmetricKeyId.Create("asymmetric-key-id"),
                CryptoVersion.Create(1));

            Assert.Throws<UserAuthenticatorException>(() => user.AddSrpAuthenticator(
                user.Login,
                Verificator.Create("verificator2"),
                Salt.Create("salt2"),
                SrpVersion.Create(2),
                CredentialBlob.Create("blob2"),
                CryptoVersion.Create(2),
                AsymmetricKeyId.Create("asymmetric-key-id-2"),
                CryptoVersion.Create(2)));
        }

        #endregion

        #region Create - AddEmailAuthenticator

        [Fact]
        public void User_AddEmailAuthenticator_ValidEmail_AddsAuthenticator()
        {
            User user = CreateUser();
            Email email = Email.Create("new-email@email.com");

            user.AddEmailAuthenticator(email);

            Assert.Single(user.UserAuthenticators);
            Assert.Contains(user.UserAuthenticators, a => a.Method == UserAuthenticatorType.Email);
        }

        #endregion

        #region Create - ChangeEmailAuthenticator 

        [Fact]
        public void User_ChangeEmailAuthenticator_ValidEmail_ReplacesAuthenticator()
        {
            User user = CreateUser();
            Email oldEmail = Email.Create("old-email@email.com");
            Email newEmail = Email.Create("new-email@email.com");

            user.AddEmailAuthenticator(oldEmail);
            user.ChangeEmailAuthenticator(newEmail);

            Assert.Single(user.UserAuthenticators);
            Assert.Contains(user.UserAuthenticators, a => a.Method == UserAuthenticatorType.Email);
        }

        #endregion

        #region Create - AddMainDek 

        [Fact]
        public void User_AddMainDek_ValidValues_AddsDek()
        {
            User user = CreateUser();

            user.AddMainDek(
                EncryptedValue.Create("encrypted"),
                Salt.Create("salt"),
                CryptoVersion.Create(1));

            Assert.Single(user.Deks);
            Assert.Contains(user.Deks, d => d.Type == DekType.Main);
        }

        [Fact]
        public void User_AddMainDek_DuplicateMainDek_ThrowsException()
        {
            User user = CreateUser();

            user.AddMainDek(
                EncryptedValue.Create("encrypted"),
                Salt.Create("salt"),
                CryptoVersion.Create(1));

            Assert.Throws<DekException>(() => user.AddMainDek(
                EncryptedValue.Create("encrypted2"),
                Salt.Create("salt2"),
                CryptoVersion.Create(2)));
        }

        #endregion

        #region Create - AddRecoveryKey 

        [Fact]
        public void User_AddRecoveryKey_ValidValues_AddsKey()
        {
            User user = CreateUser();

            user.AddRecoveryKey(
                EncryptedValue.Create("encrypted"),
                CryptoVersion.Create(1),
                KeyHint.Create("hint"));

            Assert.Single(user.RecoveryKeys);
        }

        #endregion

        #region Create - ChangeAvatar

        [Fact]
        public void User_ChangeAvatar_ValidKey_ChangesAvatar()
        {
            User user = CreateUser();
            S3Key key = S3Key.Create("Bucket", ["user", "avatars"], "avatar.svg");

            user.ChangeAvatar(key);

            Assert.Equal(key, user.AvatarKey);
        }

        #endregion

        #region Create - Delete

        [Fact]
        public void User_Delete_AddsAccountDeletedEvent()
        {
            User user = CreateUser();

            user.Delete();

            Assert.Contains(user.DomainEvents, e => e is UserAccountDeletedDomainEvent);
        }

        #endregion

        #region Create - GetChangeEmailCode

        [Fact]
        public void User_GetChangeEmailCode_ValidEmail_ReturnsSixDigitCode()
        {
            User user = CreateUser();
            Email newEmail = Email.Create("new-email@email.com");

            string code = user.GetChangeEmailCode(newEmail);

            Assert.Equal(6, code.Length);
            Assert.True(code.All(char.IsDigit));
        }

        [Fact]
        public void User_GetChangeEmailCode_ValidEmail_AddsChangeEmailRequestedEvent()
        {
            User user = CreateUser();
            Email newEmail = Email.Create("new-email@email.com");

            user.GetChangeEmailCode(newEmail);

            Assert.Contains(user.DomainEvents, e => e is ChangeEmailRequestedDomainEvent);
        }

        #endregion

        #region Create - ChangeEmail

        [Fact]
        public void User_ChangeEmail_ValidEmail_ChangesEmail()
        {
            User user = CreateUser();
            Email newEmail = Email.Create("new-email@email.com");
            DateTime oldDateUpdate = user.DateUpdate;

            user.ChangeEmail(newEmail);

            Assert.Equal(newEmail, user.Email);
            Assert.True(user.DateUpdate > oldDateUpdate);
        }

        #endregion

        #region Create - GetResetPasswordCode

        [Fact]
        public void User_GetResetPasswordCode_ReturnsSixDigitCode()
        {
            User user = CreateUser();

            string code = user.GetResetPasswordCode();

            Assert.Equal(6, code.Length);
            Assert.True(code.All(char.IsDigit));
        }

        [Fact]
        public void User_GetResetPasswordCode_AddsPasswordResetRequestedEvent()
        {
            User user = CreateUser();

            user.GetResetPasswordCode();

            Assert.Contains(user.DomainEvents, e => e is PasswordResetRequestedDomainEvent);
        }

        #endregion

        #region Create - UpdateLastEntryDate

        [Fact]
        public void User_UpdateLastEntryDate_SetsDateEntry()
        {
            User user = CreateUser();

            user.UpdateLastEntryDate();

            Assert.NotNull(user.DateEntry);
            Assert.True(user.DateEntry <= DateTime.UtcNow);
        }

        #endregion
    }
}