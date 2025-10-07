using System.Text.RegularExpressions;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.User
{
    public sealed record Phone
    {
        public string Value { get; }

        public Phone(string value) => Value = value;

        private static readonly Regex PhoneRegex = new(@"^\+?[0-9\s-]{7,20}$");

        public static Phone Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Номер телефона не может быть пустым.", nameof(phoneNumber));

            var sanitizedPhone = phoneNumber.Trim();

            if (!PhoneRegex.IsMatch(sanitizedPhone))
                throw new ArgumentException("Неверный формат номера телефона.", nameof(phoneNumber));

            return new Phone(sanitizedPhone);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Value))
                return string.Empty;

            return Value;
        }

        public static implicit operator string(Phone phone) => phone.Value;
    }
}