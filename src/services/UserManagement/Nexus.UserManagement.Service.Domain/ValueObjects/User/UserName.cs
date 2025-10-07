namespace Nexus.UserManagement.Service.Domain.ValueObjects.User
{
    public sealed record UserName
    {
        public const int MIN_LENGTH = 2;
        public const int MAX_LENGTH = 100;

        public string Value { get; }

        public UserName(string value) => Value = value;

        public static UserName Create(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Имя пользователя не может быть пустым.", nameof(userName));

            var trimmedUserName = userName.Trim();

            if (trimmedUserName.Length < MIN_LENGTH || trimmedUserName.Length > MAX_LENGTH)
                throw new ArgumentException($"Длина имени пользователя должна быть от {MIN_LENGTH} до {MAX_LENGTH} символов.", nameof(userName));

            return new UserName(trimmedUserName);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Value))
                return string.Empty;

            return Value;
        }

        public static implicit operator string(UserName userName) => userName.Value;
    }
}