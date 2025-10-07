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

        public Guid IdStatus { get; private set; }
        public Guid IdRole { get; private set; }
        public Guid? IdGender { get; private set; }
        public Guid? IdCountry { get; private set; }

        /*--Навигационные свойства--*/

        public Role Role { get; private set; } = null!;
        public Status Status { get; private set; } = null!;
        public Gender? Gender { get; private set; }
        public Country? Country { get; private set; }

        private User() { }

        private User(UserId id, Login login, UserName userName, PasswordHash passwordHash, Email email, Guid statusId, Guid roleId)
            : base(id)
        {
            Login = login;
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
            IdRole = roleId;

            DateRegistration = DateTime.UtcNow;
            DateUpdate = DateTime.UtcNow;
            IdStatus = statusId;
        }

        public static User Create(
            string login, string userName, string passwordHash, string email, string? phone,
            Guid statusId, Guid roleId, Guid? genderId, Guid? countryId)
        {
            var loginVo = Login.Create(login);
            var userNameVo = UserName.Create(userName);
            var passwordHashVo = PasswordHash.Create(passwordHash);
            var emailVo = Email.Create(email);

            var user = new User(UserId.New(), loginVo, userNameVo, passwordHashVo, emailVo, statusId, roleId);

            if (phone is not null)
                user.Phone = Phone.Create(phone);

            if (genderId.HasValue)
                user.IdGender = genderId;

            if (countryId.HasValue)
                user.IdCountry = countryId;

            //user.AddDomainEvent(new UserCreatedEvent(Guid.NewGuid(), user.Id, user.Email, user.UserName, DateTime.UtcNow));

            return user;
        }

        public void ChangeUserName(UserName userName, Guid? changedByUserId)
        {
            if (userName != UserName)
            {
                var oldUserName = UserName;
                UserName = UserName.Create(userName);

                //AddDomainEvent(new UserChangedUserNameEvent(Guid.NewGuid(), DateTime.UtcNow, Id, UserName, oldUserName, changedByUserId));
            }
        }

        public void UpdatePassword(PasswordHash passwordHash)
        {
            PasswordHash = passwordHash;

            //AddDomainEvent(new UserUpdatePasswordEvent(Guid.NewGuid(), DateTime.UtcNow, Id));
        }

        public void UpdateLastEntryDate() => DateEntry = DateTime.UtcNow;
    }
}