using Nexus.UserManagement.Service.Domain.Exceptions;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator
{
    public readonly record struct IdentityIdentifier
    {
        public string Value { get; }

        private IdentityIdentifier(string value) => Value = value;

        public static IdentityIdentifier Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new EmptyValueExeption(new Error(ErrorCode.Epmpty, "Идентификатор аунтетификации не может быть пустым."));

            return new IdentityIdentifier(value);
        }

        public override string ToString() => Value;

        public static implicit operator string(IdentityIdentifier value) => value.Value;
    }
}