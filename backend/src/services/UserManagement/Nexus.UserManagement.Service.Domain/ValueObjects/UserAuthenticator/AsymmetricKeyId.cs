using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Exceptions;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator
{
    public readonly record struct AsymmetricKeyId
    {
       public string Value { get; }

        private AsymmetricKeyId(string value)
        {
            Value = value;
        }

        public static AsymmetricKeyId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EmptyValueException(new Error(ErrorCode.Empty, "Идентификатор ассиметричного ключа обязателен."));

            return new AsymmetricKeyId(value);
        }

        public override string ToString() => Value;

        public static implicit operator string(AsymmetricKeyId value) => value.Value;  
    }
}