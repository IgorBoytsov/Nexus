using System.Text.RegularExpressions;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.User
{
    public sealed record Email
    {
        public string Value { get; } = null!;

        internal Email(string value) => Value = value;

        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не может быть пустым.", nameof(email));

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Неверный формат Email.", nameof(email));

            return new Email(email);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Value))
                return string.Empty;

            return Value;
        }

        public static implicit operator string(Email email) => email.Value;
    }
}