namespace Nexus.UserManagement.Service.Domain.ValueObjects.User
{
    public sealed record PasswordHash
    {
        public string Value { get; }

        internal PasswordHash(string value) => Value = value;

        public static PasswordHash Create(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Хеш пароля не может быть пустым.", nameof(passwordHash));

            return new PasswordHash(passwordHash);
        }

        public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;
    }
}