namespace Nexus.UserManagement.Service.Domain.ValueObjects.Country
{
    public sealed record CountryName
    {
        public const int MIN_LENGTH = 2;
        public const int MAX_LENGTH = 100;

        public string Value { get; }

        internal CountryName(string value) => Value = value;

        public static CountryName Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название страны не может быть пустым.", nameof(name));

            var trimmedUserName = name.Trim();

            if (trimmedUserName.Length < MIN_LENGTH || trimmedUserName.Length > MAX_LENGTH)
                throw new ArgumentException($"Длина страны должна быть от {MIN_LENGTH} до {MAX_LENGTH} символов.", nameof(name));

            return new CountryName(trimmedUserName);
        }

        public static implicit operator string(CountryName countryName) => countryName.Value;
    }
}