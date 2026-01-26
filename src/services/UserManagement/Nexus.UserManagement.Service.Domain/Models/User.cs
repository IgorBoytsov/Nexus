using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Exceptions;
using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Shared.Kernel.Primitives;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class User : AggregateRoot<UserId>
    {
        /*--Основная информация о пользователе--*/

        public Login Login { get; private set; } = null!;
        public UserName UserName { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public Phone? Phone { get; private set; }

        /*--Даты--*/

        public DateTime DateRegistration { get; private set; }
        public DateTime? DateEntry { get; private set; }
        public DateTime DateUpdate { get; private set; }

        /*--Связанные данные--*/

        public Guid? IdStatus { get; private set; }
        public Guid? IdGender { get; private set; }
        public Guid? IdCountry { get; private set; }

        /*--Навигационные свойства--*/

        //public UserCredentials Credentials { get; private set; } = null!;

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
            string login, string userName, /*string passwordHash, string clientSalt, string encryptedDek,*/
            string email, string? phone,
            Guid statusId, Guid? genderId, Guid? countryId)
        {
            var loginVo = Login.Create(login);
            var userNameVo = UserName.Create(userName);
            //var passwordHashVo = PasswordHash.Create(passwordHash);
            var emailVo = Email.Create(email);

            var user = new User(UserId.New(), loginVo, userNameVo, emailVo, statusId);

            //user.Credentials = new UserCredentials(user.Id, passwordHashVo, clientSalt, encryptedDek);

            if (phone is not null)
                user.Phone = Phone.Create(phone);

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

        public void UpdateVerifier(string verifier)
        {
            //Credentials.UpdateVerifier(verifier);
            DateUpdate = DateTime.UtcNow;
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

        public void AddUserAuthenticator(UserAuthenticatorType method, IdentityIdentifier identifier, CredentialBlob? credentialBlob, string? salt)
        {
            if (_userAuthenticators.Any(ua => ua.Method == method))
                throw new UserAuthenticatorException(new Error(ErrorCode.Duplicate, $"Способ входа - {method} уже используеться. Повторно его добавить нельзя."));
       
            if(_userAuthenticators.Any(ua => ua.Identifier == identifier))
                throw new UserAuthenticatorException(new Error(ErrorCode.Duplicate, $"Идентификатор - {identifier} уже используеться. Повторно его добавить нельзя."));

            var userAuth = Models.UserAuthenticator.Create(this.Id, method, identifier, credentialBlob, salt);
            _userAuthenticators.Add(userAuth);

            DateUpdate = DateTime.UtcNow;
        }

        #endregion

        #region UserSecurityAssets

        public void AddUserSecurityAssets(AssetType assetType, EncryptedAssetValue encryptedAssetValue, EncryptionMetadata encryptionMetadata)
        {
            if (assetType == AssetType.MainDek && _userSecurityAssets.Any(us => us.AssetType == AssetType.MainDek))
                throw new UserSecurityAssetsExeption(new Error(ErrorCode.Duplicate, "Основной ключ шифрования уже существуе. Для его смены используйтся процедуру ротации."));

            var userSecurity = Models.UserSecurityAsset.Create(this.Id, assetType, encryptedAssetValue, encryptionMetadata);
            _userSecurityAssets.Add(userSecurity);

            DateUpdate = DateTime.UtcNow;
        }

        #endregion
    }
}