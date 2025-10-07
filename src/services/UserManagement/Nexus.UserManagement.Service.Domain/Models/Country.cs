using Nexus.UserManagement.Service.Domain.ValueObjects.Country;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Models
{
    public sealed class Country : Entity<Guid>
    {
        public CountryName Name { get; private set; } = null!;

        private Country() { }

        private Country(Guid id, CountryName name) : base(id)
        {
            Name = name;
        }

        public static Country Create(string name)
        {
            var genderName = CountryName.Create(name);

            return new Country(Guid.NewGuid(), genderName);
        }

        public void UpdateName(CountryName countryName)
        {
            if (Name != countryName)
            {
                Name = countryName;
            }
        }
    }
}