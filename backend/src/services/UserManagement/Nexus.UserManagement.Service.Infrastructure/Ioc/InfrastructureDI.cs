using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Infrastructure.Persistence;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Countries;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Genders;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Roles;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Statuses;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Npgsql;
using Shared.Dapper.TypeHandlers;
using Shared.Redis;

namespace Nexus.UserManagement.Service.Infrastructure.Ioc
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddCashService(configuration);
            services.AddDbContext<UserManagementContext>(option => option.UseNpgsql(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IDbConnection>(sp => new NpgsqlConnection(connectionString));

            #region DapperHandler

            SqlMapper.ResetTypeHandlers();
            SqlMapper.AddTypeHandler(new JsonListStringHandler());

            #endregion

            #region Repositories
           
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

            #endregion

            return services;
        }
    }
}