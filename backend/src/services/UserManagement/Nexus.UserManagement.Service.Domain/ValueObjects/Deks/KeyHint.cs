using Crossdyne.Toolkit.Results;
using Shared.Kernel.Exceptions;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.Deks
{
    public readonly record struct KeyHint
    {
        public string Value { get; }
    
        private KeyHint(string value) => Value = value;

        public static KeyHint Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(new Error(ErrorCode.Empty, "Key hint не должен быть пустым."));

            return new KeyHint(value.Trim().ToUpper());
        }

         public override string ToString() => Value.ToString();

        public static implicit operator string(KeyHint value) => value.Value;
    }
}