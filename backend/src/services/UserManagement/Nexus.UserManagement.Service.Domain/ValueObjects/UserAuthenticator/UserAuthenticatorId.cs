namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator
{
    public readonly record struct UserAuthenticatorId
    {
        public readonly Guid Value { get; }

        private UserAuthenticatorId(Guid value) => Value = value;

        public static UserAuthenticatorId From(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Id способа аунтетификации не может быть пустым.", nameof(value));

            return new UserAuthenticatorId(value);
        }

        public static UserAuthenticatorId New() => new(Guid.NewGuid());

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(UserAuthenticatorId value) => value.Value;
    }
}