namespace Nexus.UserManagement.Service.Application.Abstractions.Models
{
    public sealed class UserView
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public DateTime DateRegistration { get; set; }
        public int IdStatus { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid IdRole { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public Guid? IdGender { get; set; }
        public string? GenderName { get; set; }
        public Guid? IdCountry { get; set; }
        public string? CountryName { get; set; }
    }
}