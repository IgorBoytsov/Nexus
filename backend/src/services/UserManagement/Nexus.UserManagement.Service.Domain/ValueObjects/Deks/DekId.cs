namespace Nexus.UserManagement.Service.Domain.ValueObjects.Deks
{
    public readonly record struct DekId
    {
        public Guid Value { get; }

        private DekId(Guid value) => Value = value;

        public static DekId From(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Идентификатор Data Encrypted Key не должен быть пустым.", nameof(value));

            return new DekId(value);
        }

        public static DekId New() => new(Guid.NewGuid());

        public static implicit operator Guid(DekId userId) => userId.Value;

        public override string ToString() => Value.ToString();
    }
}