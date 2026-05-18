namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator
{
    public readonly record struct SrpVersion
    {
        public int Value { get; }

        private SrpVersion(int value)
        {
            Value = value;
        }

        public static SrpVersion Create(int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            return new SrpVersion(value);
        }

        public override string ToString() => Value.ToString();

        public static implicit operator string(SrpVersion value) => value.ToString();
    }
}