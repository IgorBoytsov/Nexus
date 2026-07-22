using System.Data;
using Confluent.Kafka;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nexus.UserManagement.Service.Application.Events;
using Nexus.UserManagement.Service.Application.Interfaces.Clients;
using Nexus.UserManagement.Service.Application.Interfaces.Events;
using Nexus.UserManagement.Service.Application.Interfaces.Outbox;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Application.Interfaces.Transactions;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Infrastructure.Clients;
using Nexus.UserManagement.Service.Infrastructure.MessageBroker;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Extensions.Dapper;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Countries;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Genders;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Roles;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Statuses;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Npgsql;
using Shared.Contracts.Messaging.Interfaces;
using Shared.Dapper.TypeHandlers;
using Shared.Messaging;
using Shared.Redis;

namespace Nexus.UserManagement.Service.Infrastructure.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddCashService(configuration);
            services.AddDbContext<UserManagementContext>(option => option.UseNpgsql(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IDbConnection>(sp => new NpgsqlConnection(connectionString));
            services.AddScoped<ITransactionManager, EfTransactionManager>();

            services.Configure<ProducerConfig>(configuration.GetSection("Kafka:Producer"));
            services.AddSingleton(sp => new ProducerBuilder<string, string>(sp.GetRequiredService<IOptions<ProducerConfig>>().Value).Build());
            services.AddSingleton<IEventPublisher, KafkaProducer>();
            services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
            services.AddSingleton<ITopicResolver, TopicResolver>();
            
            #region Outbox

            services.AddScoped<IDbContextOutbox, DbContextOutbox>();
            services.AddSingleton<IOutboxSignal, OutboxSignal>();
            services.AddHostedService<OutboxProcessor>();

            #endregion

            #region DapperHandler

            SqlMapper.ResetTypeHandlers();
            SqlMapper.AddTypeHandler(new JsonListStringHandler());
            SqlMapper.AddTypeHandler(new S3KeyResponseTypeHandler());

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

            #region Http Clients

            string? fileServiceUrl = configuration["Urls:FileService"];
            services.AddHttpClient<IFileService, FileService>(client => client.BaseAddress = new Uri(fileServiceUrl!));

            #endregion

            return services;
        }
    }
}