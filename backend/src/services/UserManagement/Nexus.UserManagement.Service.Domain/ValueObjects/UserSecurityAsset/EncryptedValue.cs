using Nexus.UserManagement.Service.Domain.Exceptions;
using Crossdyne.Toolkit.Results;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset
{
    public readonly record struct EncryptedValue
    {
        public string Value { get; }

        private EncryptedValue(string value) => Value = value;

        public static EncryptedValue Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EmptyValueException(new Error(ErrorCode.Empty, "Зашифрованное значение не может быть пустым"));

            return new EncryptedValue(value);
        }

        public override string ToString() => Value;

        public static implicit operator string(EncryptedValue value) => value.Value;
    }
}