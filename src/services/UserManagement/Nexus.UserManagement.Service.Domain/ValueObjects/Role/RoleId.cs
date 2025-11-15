namespace Nexus.UserManagement.Service.Domain.ValueObjects.Role
{
    public readonly record struct RoleId
    {
        public Guid Value { get; }

        private RoleId(Guid value) => Value = value;

        public static RoleId From(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Id роли не может быть пустым.", nameof(value));

            return new RoleId(value);
        }

        public static RoleId New() => new(Guid.NewGuid());

        public static implicit operator Guid(RoleId userId) => userId.Value;

        public override string ToString() => Value.ToString();
    }
}