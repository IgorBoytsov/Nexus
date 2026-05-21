namespace Nexus.UserManagement.Service.Domain.ValueObjects.Deks
{
    public readonly record struct DekType
    {
        public int Value { get; }
        public string Name { get; }

        public static readonly DekType Main = new(1, nameof(Main));
        public static readonly DekType Recovery = new(2, nameof(Recovery));

        public static IReadOnlyList<DekType> All => [Main, Recovery];

        private DekType(int value, string name)
        {
            if (value <= 0) 
                throw new ArgumentException("DekType value must be positive.", nameof(value));
            
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentException("DekType name cannot be null or empty.", nameof(name));

            Value = value;
            Name = name;
        }

        public static DekType FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            var isType = All.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (!isType)
                throw new ArgumentException($"Unknown DekType name: '{name}'", nameof(name));

            return All.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static DekType FromValue(int value)
        {
            var isType = All.Any(t => t.Value == value);

            if (!isType)
                throw new ArgumentException($"Unknown DekType value: {value}", nameof(value));

            return All.FirstOrDefault(t => t.Value == value);
        }

        public override string ToString() => Name;
    }
}