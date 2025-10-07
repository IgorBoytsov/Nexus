using Nexus.UserManagement.Service.Domain.ValueObjects.Gender;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class Gender : Entity<Guid>
    {
        public GenderName Name { get; private set; } = null!;

        private Gender() { }

        private Gender(Guid id, GenderName name) : base(id)
        {
            Name = name;
        }

        public static Gender Create(string name)
        {
            var genderName = GenderName.Create(name);

            return new Gender(Guid.NewGuid(), genderName);
        }

        public void UpdateName(GenderName genderName)
        {
            if (genderName !=  Name)
                Name = genderName;
        }
    }
}