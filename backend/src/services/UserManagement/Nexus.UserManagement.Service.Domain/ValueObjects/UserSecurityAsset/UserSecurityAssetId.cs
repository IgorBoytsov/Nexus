namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset
{
    public readonly record struct UserSecurityAssetId
    {
        public readonly Guid Value { get; }

        private UserSecurityAssetId(Guid value) => Value = value;

        public static UserSecurityAssetId From(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"Id {nameof(UserSecurityAssetId)} не может быть пустым.", nameof(value));

            return new UserSecurityAssetId(value);
        }

        public static UserSecurityAssetId New() => new(Guid.NewGuid());

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(UserSecurityAssetId value) => value.Value;
    }
}