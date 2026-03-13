namespace Nexus.UserManagement.Service.Domain.ValueObjects.Role
{
    public sealed record RoleName
    {
        public string Value { get; }

        internal RoleName(string value) => Value = value;

        public static RoleName Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название роли не может быть пустым.", nameof(name));

            return new RoleName(name);
        }

        public static implicit operator string(RoleName roleName) => roleName.Value;
    }
}