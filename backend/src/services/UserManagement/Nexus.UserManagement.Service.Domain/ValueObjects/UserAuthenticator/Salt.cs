using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Exceptions;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator
{
    public readonly record struct Salt
    {
        public string Value { get; }

        private Salt(string value) => Value = value;

        public static Salt Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EmptyValueException(new Error(ErrorCode.Empty, "Бы передана пустая соль"));

            return new Salt(value);
        }
 
        public override string ToString() => Value;

        public static implicit operator string(Salt value) => value.Value;
    }
}