namespace Nexus.UserManagement.Service.Domain.SmartEnums
{
    public class EnumRole
    {
        public Guid Id { get; }
        public string Name { get; }

        private EnumRole(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static readonly EnumRole User = new(new Guid("9e04b60f-b2d4-4840-a0de-ae9d0a35be26"), "User");

        public static readonly EnumRole Admin = new(new Guid("74dbe328-5f70-439d-b287-f1ad18e65473"), "Admin");

        public static EnumRole FromId(Guid id)
        {
            if (id == User.Id) return User;
            if (id == Admin.Id) return Admin;

            throw new KeyNotFoundException($"Роль с Id '{id}' не найдена.");
        }
    }
}