using System.Text.RegularExpressions;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.User
{
    public sealed record Login
    {
        public const int MIN_LENGTH = 3;
        public const int MAX_LENGTH = 50;

        public string Value { get; }

        internal Login(string value) => Value = value;

        private static readonly Regex AllowedCharsRegex = new("^[a-zA-Z0-9_]+$");

        public static Login Create(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Логин не может быть пустым.", nameof(login));

            if (login.Length < MIN_LENGTH || login.Length > MAX_LENGTH)
                throw new ArgumentException($"Длина логина должна быть от {MIN_LENGTH} до {MAX_LENGTH} символов.", nameof(login));

            if (!AllowedCharsRegex.IsMatch(login))
                throw new ArgumentException("Логин содержит недопустимые символы.", nameof(login));

            return new Login(login);
        }

        public override string ToString() => Value;

        public static implicit operator string(Login login) => login.Value;
    }
}