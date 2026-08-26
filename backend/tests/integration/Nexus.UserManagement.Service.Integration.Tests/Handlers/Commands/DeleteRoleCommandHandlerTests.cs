using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Roles.Commands.Delete;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Roles;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class DeleteRoleCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private readonly UserManagementContext _context;
    private readonly RoleRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly OutboxSignal _outboxSignal;
    private readonly DeleteRoleCommandHandler _handler;
    private readonly CancellationToken _ct = default;

    public DeleteRoleCommandHandlerTests(TestFixture fixture)
    {
        _fixture = fixture;

        _context = fixture.CreateDbContext();
        _repo = new RoleRepository(_context);
        _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
        _uow = fixture.CreateUnitOfWork(_context);

        _handler = new DeleteRoleCommandHandler(_uow, _repo);
    }

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

    public async ValueTask DisposeAsync()
    {
        _outboxSignal.Dispose();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task Handle_ExistingId_ShouldDeleteCountry_And_ReturnSuccess()
    {
        var role = Role.Create("Admin");
        await _context.Roles.AddAsync(role, _ct);
        await _context.SaveChangesAsync(_ct);

        var command = new DeleteRoleCommand(role.Id);
        var result = await _handler.Handle(command, _ct);

        result.IsSuccess.Should().BeTrue();

        var deleted = await _context.Countries.AsNoTracking().FirstOrDefaultAsync(r => r.Id == role.Id, _ct);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NonExistingId_ShouldReturnFailure()
    {
        var command = new DeleteRoleCommand(Guid.NewGuid());
        var result = await _handler.Handle(command, _ct);

        result.IsSuccess.Should().BeFalse();
    }
    }
}