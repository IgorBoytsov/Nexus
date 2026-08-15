using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeEmail;
using Nexus.UserManagement.Service.Application.Interfaces.UnitOfWork;
using Nexus.UserManagement.Service.Domain.Enums;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Shared.Test.Cache;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class ChangeEmailCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly InMemoryCacheService _cache;
        private readonly ChangeEmailCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        private string _code = "724014";

        public ChangeEmailCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);
            _cache = new InMemoryCacheService();

            _handler = new ChangeEmailCommandHandler(_repo, _uow, _cache);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ExistingId_ShouldUpdateEmail()
        {
            _fixture.FileServiceMock.Reset();

            var user = User.Create(
                Login.Create("TestLogin"),
                UserName.Create("TestUserName"),
                Email.Create("valid@email.com"),
                statusId: Guid.NewGuid(),
                genderId: Guid.NewGuid(),
                countryId: Guid.NewGuid());

            await _context.Users.AddAsync(user, _ct);
            await _context.SaveChangesAsync(_ct);

            await _cache.SetStringAsync($"ConfirmCode for {user.Login.Value.ToLowerInvariant()}", _code, TimeSpan.FromMinutes(5));
            
            var command = new ChangeEmailCommand(user.Id, "new@email.com", _code);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();

            var updated = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserAuthenticators)
                .FirstOrDefaultAsync(u => u.Id == user.Id, _ct);

            updated.Should().NotBeNull();
            updated!.Email.Value.Should().Be("new@email.com");
            updated!.UserAuthenticators.FirstOrDefault(ua => ua.Method == UserAuthenticatorType.Email);
        }

        [Fact]
        public async Task Handle_NonExistingUserId_ShouldReturnFailureNotFoundCode()
        {
            var command = new ChangeEmailCommand(Guid.NewGuid(), "new@email.com", _code);
            var result = await _handler.Handle(command, _ct);
            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}