namespace Nexus.UserManagement.Service.Domain.ValueObjects.RecoveryKeys
{
    public readonly record struct RecoveryKeyId
    {
        public Guid Value { get; }

        private RecoveryKeyId(Guid value) => Value = value;

        public static RecoveryKeyId From(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Идентификатор ключа восстановления не должен быть пустым.", nameof(value));

            return new RecoveryKeyId(value);
        }

        public static RecoveryKeyId New() => new(Guid.NewGuid());

        public static implicit operator Guid(RecoveryKeyId userId) => userId.Value;

        public override string ToString() => Value.ToString();
    }
}