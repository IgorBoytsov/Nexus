using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Application.Abstractions.Models;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Configurations.Read;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts
{
    public sealed class ReadContext(DbContextOptions<ReadContext> options) : DbContext(options), IReadDbContext
    {
        private DbSet<UserView> _users { get; set; } = null!;
        private DbSet<RoleView> _roles { get; set; } = null!;
        private DbSet<StatusView> _statuses { get; set; } = null!;
        private DbSet<GenderView> _genders { get; set; } = null!;
        private DbSet<CountryView> _countries { get; set; } = null!;

        public IQueryable<UserView> UsersView => _users;
        public IQueryable<RoleView> RolesView => _roles;
        public IQueryable<StatusView> StatusesView => _statuses;
        public IQueryable<GenderView> GendersView => _genders;
        public IQueryable<CountryView> CountriesView => _countries;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserReadConfiguration());
            modelBuilder.ApplyConfiguration(new RoleReadConfiguration());
            modelBuilder.ApplyConfiguration(new StatusReadConfiguration());
            modelBuilder.ApplyConfiguration(new GenderReadConfiguration());
            modelBuilder.ApplyConfiguration(new CountryReadConfiguration());
        }
    }
}