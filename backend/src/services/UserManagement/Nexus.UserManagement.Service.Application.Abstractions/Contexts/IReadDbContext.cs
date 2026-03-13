using Nexus.UserManagement.Service.Application.Abstractions.Models;

namespace Nexus.UserManagement.Service.Application.Abstractions.Contexts
{
    public interface IReadDbContext
    {
        public IQueryable<UserView> UsersView { get; }
        public IQueryable<RoleView> RolesView { get; }
        public IQueryable<StatusView> StatusesView { get; }
        public IQueryable<GenderView> GendersView { get; }
        public IQueryable<CountryView> CountriesView { get; }
    }
}