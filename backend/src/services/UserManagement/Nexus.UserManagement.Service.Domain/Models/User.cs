using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Exceptions;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Crossdyne.Toolkit.Results;
using Shared.Kernel.Errors;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class User : AggregateRoot<UserId>
    {
        /*--Основная информация о пользователе--*/

        public Login Login { get; private set; } 
        public UserName UserName { get; private set; }
        public Email Email { get; private set; }

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

        private readonly List<UserAuthenticator> _userAuthenticators = [];
        public IReadOnlyCollection<UserAuthenticator> UserAuthenticators => _userAuthenticators.AsReadOnly();

        private readonly List<UserSecurityAsset> _userSecurityAssets = [];
        public IReadOnlyCollection<UserSecurityAsset> UserSecurityAssets => _userSecurityAssets.AsReadOnly();

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
            string login, string userName,
            string email,
            Guid statusId, Guid? genderId, Guid? countryId)
        {
            var loginVo = Login.Create(login);
            var userNameVo = UserName.Create(userName);
            var emailVo = Email.Create(email);

            var user = new User(UserId.New(), loginVo, userNameVo, emailVo, statusId);

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
                UserName = UserName.Create(userName);
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

        #region UserAuthenticators

        public void AddSrpAuthenticator(Login login, Verificator encryptedVerifier, Salt salt, SrpVersion srpVersion, CredentialBlob encryptedVerifierWrapKey, CryptoVersion keyWrapVersion, AsymmetricKeyId asymmetricKeyId)
        {
            EnsureNoAuthenticatorOfType(UserAuthenticatorType.SRP);
            var srp = SrpAuthenticator.Create(this.Id, login, encryptedVerifier, salt, srpVersion, encryptedVerifierWrapKey, keyWrapVersion, asymmetricKeyId);
            _userAuthenticators.Add(srp);

            MarkUpdate();
        }

        public void UpdateSrpAuthenticator( Verificator encryptedVerifier, Salt salt, SrpVersion srpVersion, CredentialBlob encryptedVerifierWrapKey, CryptoVersion keyWrapVersion, AsymmetricKeyId asymmetricKeyId)
        {
            var srp = GetAuthenticator<SrpAuthenticator>(UserAuthenticatorType.SRP);
            srp?.Update(encryptedVerifier, salt, srpVersion, encryptedVerifierWrapKey, keyWrapVersion, asymmetricKeyId);

            MarkUpdate(); 
        }

        public void AddEmailAuthenticator(Email email)
        {
            EnsureNoAuthenticatorOfType(UserAuthenticatorType.Email);
            var emailAuth = EmailAuthenticator.Create(this.Id, email);
            _userAuthenticators.Add(emailAuth);

            MarkUpdate();
        }

        public void UpdateEmailAuthenticator(Email email)
        {
            var emailAuth = GetAuthenticator<EmailAuthenticator>(UserAuthenticatorType.Email);
            emailAuth?.Update(email);

            MarkUpdate();
        }

        public void ActivatedAuthenticator(UserAuthenticatorType type)
        {
            var auth = GetAuthenticator(type);
            auth?.Activate();
        }

        public void DeactivateAuthenticator(UserAuthenticatorType type)
        {
            var auth = GetAuthenticator(type);
            auth?.Deactivate();
        }

        #endregion

        #region UserSecurityAssets

        public void AddUserSecurityAssets(AssetType assetType, EncryptedAssetValue encryptedAssetValue, int cryptoVersion)
        {
            if (assetType == AssetType.MainDek && _userSecurityAssets.Any(us => us.AssetType == AssetType.MainDek))
                throw new UserSecurityAssetsException(new Error(AppErrors.Duplicate, "Основной ключ шифрования уже существует. Для его смены используйте процедуру ротации."));

            var userSecurity = UserSecurityAsset.Create(this.Id, assetType, encryptedAssetValue, cryptoVersion);
            _userSecurityAssets.Add(userSecurity);

            DateUpdate = DateTime.UtcNow;
        }

        public void UpdateMainDek(EncryptedAssetValue encryptedAssetValue, int cryptoVersion)
        {
            var dek = _userSecurityAssets.FirstOrDefault(x => x.AssetType == AssetType.MainDek);

            dek?.UpdateMainDek(encryptedAssetValue, cryptoVersion);
        }

        public void ClearRecoveryKeys()
        {
            _userSecurityAssets.RemoveAll(x => x.AssetType == AssetType.RecoveryKey);
            MarkUpdate();
        }

        #endregion

        #region Public Helpers

        private T? GetAuthenticator<T>(UserAuthenticatorType type) where T : UserAuthenticator
            => _userAuthenticators.OfType<T>().FirstOrDefault(a => a.Method == type && a.IsActive);
            
        private UserAuthenticator? GetAuthenticator(UserAuthenticatorType type)
            => _userAuthenticators.FirstOrDefault(a => a.Method == type && a.IsActive);


        public void MarkUpdate() => DateUpdate = DateTime.UtcNow;

        #endregion

        #region Private Helpers

        private void EnsureNoAuthenticatorOfType(UserAuthenticatorType type)
        {
            if (_userAuthenticators.Any(a => a.Method == type && a.IsActive))
                throw new UserAuthenticatorException(new Error(AppErrors.Duplicate, $"Метод аутентификации '{type}' уже добавлен."));
        }

        #endregion
    }
}