namespace Nexus.UserManagement.Service.Application.Abstractions.Models
{
    public sealed class RoleView
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}