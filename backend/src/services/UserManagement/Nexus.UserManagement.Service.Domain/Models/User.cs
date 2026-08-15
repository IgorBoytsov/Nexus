using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Events;
using Nexus.UserManagement.Service.Domain.Exceptions;
using Nexus.UserManagement.Service.Domain.ValueObjects.Common;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class User : AggregateRoot<UserId>
    {
        /*--Основная информация о пользователе--*/

        public Login Login { get; private set; } 
        public UserName UserName { get; private set; }
        public Email Email { get; private set; }

        /*--Аватар--*/

        public S3Key? AvatarKey { get; private set; }

        /*--Даты--*/

        public DateTime DateRegistration { get; private set; }
        public DateTime? DateEntry { get; private set; }
        public DateTime DateUpdate { get; private set; }

        /*--Связанные данные--*/

        public Guid? IdStatus { get; private set; }
        public Guid? IdGender { get; private set; }
        public Guid? IdCountry { get; private set; }

        /*--Навигационные свойства--*/

        private readonly List<UserRoles> _userRoles = [];
        public IReadOnlyCollection<UserRoles> UserRoles => _userRoles.AsReadOnly();

        private readonly List<Dek> _deks = [];
        public IReadOnlyCollection<Dek> Deks => _deks.AsReadOnly();

        private readonly List<UserAuthenticator> _userAuthenticators = [];
        public IReadOnlyCollection<UserAuthenticator> UserAuthenticators => _userAuthenticators.AsReadOnly();

        private readonly List<RecoveryKey> _recoveryKeys = [];
        public IReadOnlyCollection<RecoveryKey> RecoveryKeys => _recoveryKeys.AsReadOnly();

        private User() { }

        private User(UserId id, Login login, UserName userName, Email email, Guid statusId)
            : base(id)
        {
            Login = login;
            UserName = userName;
            Email = email;

            DateRegistration = DateTime.UtcNow;
            DateUpdate = DateTime.UtcNow;
            IdStatus = statusId;
        }

        public static User Create(
            Login login, UserName userName,
            Email email,
            Guid statusId, Guid? genderId, Guid? countryId)
        {
            var user = new User(UserId.New(), login, userName, email, statusId);

            if (genderId.HasValue)
                user.IdGender = genderId;

            if (countryId.HasValue)
                user.IdCountry = countryId;

            return user;
        }

        public void ChangeUserName(UserName userName, Guid? changedByUserId)
        {
            if (userName != UserName)
            {
                var oldUserName = UserName;
                UserName = userName;
            }
        }

        public void UpdateLastEntryDate() => DateEntry = DateTime.UtcNow;

        #region Roles

        public void AddRole(RoleId roleId)
        {
            if (_userRoles.Any(ur => ur.RoleId == roleId))
                return;

            _userRoles.Add(Models.UserRoles.Create(Id, roleId));
            DateUpdate = DateTime.UtcNow;
        }

        public void RemoveRole(RoleId roleId)
        {
            var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);

            if (userRole is not null)
            {
                _userRoles.Remove(userRole);
                DateUpdate = DateTime.UtcNow;
            }
        }

        #endregion

        #region AccountAccess

        public void ChangePassword(
            Verificator verificator, Salt salt, SrpVersion srpVersion, CredentialBlob credentialBlob, 
            CryptoVersion srpWrapKeyCryptoVersion, AsymmetricKeyId asymmetricKeyId, 
            EncryptedValue newEncryptedDek, Salt newDekSalt, CryptoVersion dekCryptoVersion, CryptoVersion srpCryptoVersion)
        {
            var srp = GetSrpAuthenticator();
            srp.Update(verificator, salt, srpVersion, credentialBlob, srpWrapKeyCryptoVersion, asymmetricKeyId, srpCryptoVersion);

            var mainDek = GetMainDek();
            mainDek.Rotate(newEncryptedDek, newDekSalt, dekCryptoVersion);
        }

        public void ResetPassword(
            Verificator verificator, Salt salt, SrpVersion srpVersion, 
            CredentialBlob credentialBlob, CryptoVersion srpWrapKeyCryptoVersion, AsymmetricKeyId asymmetricKeyId,
            EncryptedValue newEncryptedDek, Salt newDekSalt, CryptoVersion dekCryptoVersion, CryptoVersion srpCryptoVersion)
        {
            var srp = GetSrpAuthenticator();
            srp.Update(verificator, salt, srpVersion, credentialBlob, srpWrapKeyCryptoVersion, asymmetricKeyId, srpCryptoVersion);

            var mainDek = GetMainDek();
            mainDek.Rotate(newEncryptedDek, newDekSalt, dekCryptoVersion);
            ClearRecoveryKeys();

            DateUpdate = DateTime.UtcNow;

            AddDomainEvent(new UserPasswordResetDomainEvent(Guid.CreateVersion7(), DateTime.UtcNow, this.Id)); 
        }

        private SrpAuthenticator GetSrpAuthenticator()
        {
            return UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault() 
                ?? throw new UserAuthenticatorException(new Error(ErrorCode.NotFound, "SRP аутентификатор не найден"));
        }

        private Dek GetMainDek()
        {
            return Deks.FirstOrDefault(x => x.Type == DekType.Main) 
                ?? throw new DekException(new Error(ErrorCode.NotFound, "Основной DEK не найден"));
        }

        #endregion

        #region UserAuthentication

        public void AddSrpAuthenticator(Login login, Verificator verificator, Salt salt, SrpVersion srpVersion, CredentialBlob credentialBlob, CryptoVersion cryptoVersion, AsymmetricKeyId asymmetricKeyId, CryptoVersion srpCryptoVersion)
        {
            if (_userAuthenticators.Any(x => x.Method == UserAuthenticatorType.SRP))
                throw new UserAuthenticatorException(new Error(ErrorCode.Exist, "Метод входа через пароль уже существует для данного аккаунта"));

                _userAuthenticators.Add(SrpAuthenticator.Create(this.Id, login, verificator, salt, srpVersion, credentialBlob, cryptoVersion, asymmetricKeyId, srpCryptoVersion));
        }

        public void AddEmailAuthenticator(Email email)
        {
            _userAuthenticators.Add(EmailAuthenticator.Create(this.Id, email));
        }

        public void ChangeEmailAuthenticator(Email email)
        {
            var emailAuth = _userAuthenticators.FirstOrDefault(ua => ua.Method == UserAuthenticatorType.Email);

            if (emailAuth is not null)
                _userAuthenticators.Remove(emailAuth);

            AddEmailAuthenticator(email);
        }

        #endregion

        #region Deks
       
        public void AddMainDek(EncryptedValue encryptedValue, Salt salt, CryptoVersion cryptoVersion)
        {
            if (_deks.Any(x => x.Type == DekType.Main))
                throw new DekException(new Error(ErrorCode.Exist, "Основной ключ шифрования уже настроен."));

            _deks.Add(Dek.Create(this.Id, encryptedValue, salt, cryptoVersion, DekType.Main));
        }

        #endregion

        #region RecoveryKeys

        public void ClearRecoveryKeys() => _recoveryKeys.Clear();

        public void AddRecoveryKey(EncryptedValue encryptedValue, CryptoVersion cryptoVersion, KeyHint keyHint)
        {
            var key = RecoveryKey.Create(this.Id, encryptedValue, cryptoVersion, keyHint);
            _recoveryKeys.Add(key);
        }

        #endregion

        #region Avatar

        public void ChangeAvatar(S3Key key)
        {
            AvatarKey = key;
            DateUpdate = DateTime.UtcNow;
        }

        public void Delete()
        {
            AddDomainEvent(new UserAccountDeletedDomainEvent(Guid.CreateVersion7(), DateTime.UtcNow, this.Id));
        }

        #endregion

        public string GetChangeEmailCode(Email email)
        {
            var code = GenerateCode();

            AddDomainEvent(new ChangeEmailRequestedDomainEvent(Guid.CreateVersion7(), DateTime.UtcNow, this.Id, email, code, DateTime.UtcNow.AddMinutes(10)));
        
            return code;
        }

        public void ChangeEmail(Email email)
        {
            Email = email;
            DateUpdate = DateTime.UtcNow;
        }

        public string GetResetPasswordCode()
        {
            var code = GenerateCode();

            AddDomainEvent(new PasswordResetRequestedDomainEvent(Guid.CreateVersion7(), DateTime.UtcNow, this.Id, this.Email.Value, code, DateTime.UtcNow.AddMinutes(10)));

            return code;
        }

        private string GenerateCode()
        {
            var rnd = new Random();
            List<int> number = [];

            for (int i = 0; i < 6; i++)
            {
                number.Add(rnd.Next(0,9));
            }

            return string.Join("", number);
        }
    }
}