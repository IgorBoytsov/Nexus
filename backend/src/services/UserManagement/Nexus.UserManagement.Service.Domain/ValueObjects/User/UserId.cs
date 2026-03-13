namespace Nexus.UserManagement.Service.Domain.ValueObjects.User
{
    public readonly record struct UserId
    {
        public Guid Value { get; }

        private UserId(Guid value) => Value = value;

        public static UserId From(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Id пользователя не может быть пустым.", nameof(value));

            return new UserId(value);
        }

        public static UserId New() => new(Guid.NewGuid());

        public static implicit operator Guid(UserId userId) => userId.Value;

        public override string ToString() => Value.ToString();
    }
}