using Nexus.UserManagement.Service.Domain.Exceptions;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset
{
    public readonly record struct EncryptedAssetValue
    {
        public string Value { get; }

        private EncryptedAssetValue(string value) => Value = value;

        public static EncryptedAssetValue Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EmptyValueExeption(new Error(ErrorCode.Epmpty, "Зашифрованное значение не может быть пустым"));

            return new EncryptedAssetValue(value);
        }

        public override string ToString() => Value;

        public static implicit operator string(EncryptedAssetValue value) => value.Value;
    }
}