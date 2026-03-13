namespace Nexus.UserManagement.Service.Domain.SmartEnums
{
    public class EnumStatus
    {
        public Guid Id { get; }
        public string Name { get; }

        private EnumStatus(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static readonly EnumStatus AwaitingVerification = new(new Guid("7dab8878-9e38-4573-8909-3cf43d4308dd"), "AwaitingVerification");

        public static readonly EnumStatus Active = new(new Guid("692cef90-0d1f-4706-a397-e7c74e851660"), "Active");

        public static readonly EnumStatus Suspended = new(new Guid("2ce5b393-26cf-40a7-9518-039ee1d29ba9"), "Suspended");
        
        public static readonly EnumStatus Deactivated = new(new Guid("12f12ed0-f290-461e-a7f9-87028bf98cdc"), "Deactivated");

        public static EnumStatus FromId(Guid id)
        {
            if (id == AwaitingVerification.Id) return AwaitingVerification;
            if (id == Active.Id) return Active;
            if (id == Suspended.Id) return Suspended;
            if (id == Deactivated.Id) return Deactivated;

            throw new KeyNotFoundException($"Статуса с Id '{id}' не найдена.");
        }
    }
}