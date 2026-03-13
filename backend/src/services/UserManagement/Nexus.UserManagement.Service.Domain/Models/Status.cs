using Nexus.UserManagement.Service.Domain.ValueObjects.Status;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class Status : Entity<Guid>
    {
        public StatusName Name { get; set; } = null!;

        private Status() { }

        public Status(Guid id, StatusName name) : base(id) => Name = name;

        public static Status Create(string name)
        {
            var statusNeame = StatusName.Create(name);

            return new Status(Guid.NewGuid(), statusNeame);
        }

        public void UpdateName(StatusName statusName)
        {
            if (Name != statusName)
                Name = statusName;
        }
    }
}