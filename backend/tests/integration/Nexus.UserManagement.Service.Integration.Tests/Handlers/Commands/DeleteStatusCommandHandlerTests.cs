using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Delete;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Statuses;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class DeleteStatusCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private readonly UserManagementContext _context;
    private readonly StatusRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly OutboxSignal _outboxSignal;
    private readonly DeleteStatusCommandHandler _handler;
    private readonly CancellationToken _ct = default;

    public DeleteStatusCommandHandlerTests(TestFixture fixture)
    {
        _fixture = fixture;

        _context = fixture.CreateDbContext();
        _repo = new StatusRepository(_context);
        _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
        _uow = fixture.CreateUnitOfWork(_context);

        _handler = new DeleteStatusCommandHandler(_uow, _repo);
    }

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

    public async ValueTask DisposeAsync()
    {
        _outboxSignal.Dispose();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task Handle_ExistingId_ShouldDeleteStatus_And_ReturnSuccess()
    {
        var status = Status.Create("Active");
        await _context.Statuses.AddAsync(status, _ct);
        await _context.SaveChangesAsync(_ct);

        var command = new DeleteStatusCommand(status.Id);
        var result = await _handler.Handle(command, _ct);

        result.IsSuccess.Should().BeTrue();

        var deleted = await _context.Countries.AsNoTracking().FirstOrDefaultAsync(s => s.Id == status.Id, _ct);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NonExistingId_ShouldReturnFailure()
    {
        var command = new DeleteStatusCommand(Guid.NewGuid());
        var result = await _handler.Handle(command, _ct);

        result.IsSuccess.Should().BeFalse();
    }
    }
}