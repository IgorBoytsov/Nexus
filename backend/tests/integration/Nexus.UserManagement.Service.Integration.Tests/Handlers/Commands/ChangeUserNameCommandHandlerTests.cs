using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeUserName;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class ChangeUserNameCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly ChangeUserNameCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public ChangeUserNameCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);

            _handler = new ChangeUserNameCommandHandler(_repo, _uow);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldChangeUserName()
        {
            var user = User.Create(
                Login.Create("testlogin"),
                UserName.Create("TestUser"),
                Email.Create("old@example.com"),
                statusId: Guid.NewGuid(),
                genderId: Guid.NewGuid(),
                countryId: Guid.NewGuid());

            await _context.Users.AddAsync(user, _ct);
            await _context.SaveChangesAsync(_ct);

            var command = new ChangeUserNameCommand(user.Id, "NewTestUser");

            var result = await _handler.Handle(command, _ct);

            var userInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id, _ct);
            result.IsSuccess.Should().BeTrue();
            userInDb?.UserName.Value.Should().Be("NewTestUser");
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFound()
        {
            var command = new ChangeUserNameCommand(Guid.NewGuid(), "NewTestUser");
            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}