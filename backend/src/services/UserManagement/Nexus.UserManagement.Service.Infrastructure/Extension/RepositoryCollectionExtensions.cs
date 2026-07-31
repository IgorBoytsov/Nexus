using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Countries;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Genders;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Roles;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Statuses;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class RepositoryCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICountryReadOnlyRepository, CountryReadOnlyRepository>();

            services.AddScoped<IGenderRepository, GenderRepository>();
            services.AddScoped<IGenderReadOnlyRepository, GenderReadOnlyRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleReadOnlyRepository, RoleReadOnlyRepository>();

            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddScoped<IStatusReadOnlyRepository, StatusReadOnlyRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();

            return services;
        }
    }
}