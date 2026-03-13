namespace Nexus.UserManagement.Service.Domain.ValueObjects.Gender
{
    public sealed record GenderName
    {
        public const int MIN_LENGTH = 2;
        public const int MAX_LENGTH = 50;

        public string Value { get; }

        internal GenderName(string value) => Value = value;

        public static GenderName Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название гендера не может быть пустым.", nameof(name));

            var trimmedUserName = name.Trim();

            if (trimmedUserName.Length < MIN_LENGTH || trimmedUserName.Length > MAX_LENGTH)
                throw new ArgumentException($"Длина гендера должна быть от {MIN_LENGTH} до {MAX_LENGTH} символов.", nameof(name));

            return new GenderName(trimmedUserName);
        }

        public static implicit operator string(GenderName genderName) => genderName.Value;
    }
}