using Nexus.UserManagement.Service.Domain.Exceptions;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator
{
    public readonly record struct CredentialBlob
    {
        public string Value { get; }

        private CredentialBlob(string value) => Value = value;

        public static CredentialBlob Create(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new EmptyValueExeption(new Error(ErrorCode.Epmpty, "Данные верификации не могут быть пустыми."));

            return new CredentialBlob(value);
        }

        public override string ToString() => Value;

        public static implicit operator string(CredentialBlob value) => value.Value;
    }
}