using Nexus.UserManagement.Service.Domain.ValueObjects.Role;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class User : AggregateRoot<UserId>
    {
        /*--Основная информация о пользователе--*/

        public Login Login { get; private set; } = null!;
        public UserName UserName { get; private set; } = null!;
        public PasswordHash PasswordHash { get; private set; } = null!;
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

        private readonly List<UserRoles> _userRoles = [];
        public IReadOnlyCollection<UserRoles> UserRoles => _userRoles.AsReadOnly();

        public Status Status { get; private set; } = null!;
        public Gender? Gender { get; private set; }
        public Country? Country { get; private set; }

        private User() { }

        private User(UserId id, Login login, UserName userName, PasswordHash passwordHash, Email email, Guid statusId/*, Guid roleId*/)
            : base(id)
        {
            Login = login;
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
            //IdRole = roleId;

            DateRegistration = DateTime.UtcNow;
            DateUpdate = DateTime.UtcNow;
            IdStatus = statusId;
        }

        public static User Create(
            string login, string userName, string passwordHash, string email, string? phone,
            Guid statusId, /*Guid roleId,*/ Guid? genderId, Guid? countryId)
        {
            var loginVo = Login.Create(login);
            var userNameVo = UserName.Create(userName);
            var passwordHashVo = PasswordHash.Create(passwordHash);
            var emailVo = Email.Create(email);

            var user = new User(UserId.New(), loginVo, userNameVo, passwordHashVo, emailVo, statusId/*, roleId*/);

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

        public void UpdatePassword(PasswordHash passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public void UpdateLastEntryDate() => DateEntry = DateTime.UtcNow;

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
    }
}