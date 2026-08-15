using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nexus.UserManagement.Service.Application.Interfaces.Transactions;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Infrastructure.Persistence;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class EntityFrameworkCoreCollectionExtensions
    {
        public static IServiceCollection RegisterWriteDatabase(this IServiceCollection services, string dateBaseConnectionString)
        {
            services.AddDbContext<UserManagementContext>(option => option.UseNpgsql(dateBaseConnectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransactionManager, EfTransactionManager>();

            return services;
        } 
    }
}