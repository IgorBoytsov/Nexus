namespace Nexus.UserManagement.Service.Application.Abstractions.Models
{
    public sealed class CountryView
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}