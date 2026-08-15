namespace Nexus.UserManagement.Service.Domain.ValueObjects.Status
{
    public sealed record StatusName
    {
        public string Value { get; }

        internal StatusName(string value) => Value = value;

        public static StatusName Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название статуса не может быть пустым.", nameof(name));

            return new StatusName(name);
        }

        public static implicit operator string(StatusName statusName) => statusName.Value;
    }
}