using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Infrastructure.Persistence;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using WireMock.Server;
using Xunit;

namespace Nexus.Authentication.Service.Integration.Tests;

public class TestFixture : IAsyncLifetime
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AuthenticationContext> _options;
    public WireMockServer UserManagementServiceMock { get; } = WireMockServer.Start();

    public TestFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AuthenticationContext>().UseSqlite(_connection).Options;
    }

    public AuthenticationContext CreateDbContext() => new(_options);


    public IUnitOfWork CreateUnitOfWork(AuthenticationContext context)
    {
        return new UnitOfWork(context);
    }

    public async ValueTask InitializeAsync()
    {
        using var ctx = CreateDbContext();
        await ctx.Database.EnsureCreatedAsync();
    }

    public async ValueTask ResetDatabaseAsync()
    {
        using var ctx = CreateDbContext();
        await ctx.Database.ExecuteSqlRawAsync(
            "DELETE FROM AccessData;");
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}