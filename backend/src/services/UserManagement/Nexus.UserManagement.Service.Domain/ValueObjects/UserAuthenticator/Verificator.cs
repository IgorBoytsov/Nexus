using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Exceptions;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator
{
    public readonly record struct Verificator
    {
        public string Value { get; }

        private Verificator(string value) => Value = value;

        public static Verificator Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EmptyValueException(new Error(ErrorCode.Empty, "Была передана пустая соль"));

            return new Verificator(value);
        }
        
        public override string ToString() => Value;

        public static implicit operator string(Verificator value) => value.Value;
    }
}