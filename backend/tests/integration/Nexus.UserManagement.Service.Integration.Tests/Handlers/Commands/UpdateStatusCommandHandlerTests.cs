using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Statuses.Commands.Update;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Statuses;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class UpdateStatusCommandHandlerTests: IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly StatusRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly UpdateStatusCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public UpdateStatusCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new StatusRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);

            _handler = new UpdateStatusCommandHandler(_uow, _repo);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ExistingId_ShouldUpdatedStatus()
        {
            var status = Status.Create("Ban");
            await _context.Statuses.AddAsync(status, _ct);
            await _context.SaveChangesAsync(_ct);

            string newName = "NewName";

            var command = new UpdateStatusCommand(status.Id, newName);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();
            var updated = await _context.Statuses.FirstOrDefaultAsync(c => c.Id == status.Id, _ct);
            updated?.Name.Value.Should().Be(newName);
        }

        [Fact]
        public async Task Handle_NonExistingId_ShouldReturnFailure()
        {
            var command = new UpdateStatusCommand(Guid.NewGuid(), "NewName");
            var result = await _handler.Handle(command, _ct);
            result.IsSuccess.Should().BeFalse();
        }
    }
}