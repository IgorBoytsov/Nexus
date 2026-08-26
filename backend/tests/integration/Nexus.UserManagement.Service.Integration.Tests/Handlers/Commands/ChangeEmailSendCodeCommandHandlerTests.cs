using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeEmailSendCode;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Shared.Test.Cache;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class ChangeEmailSendCodeCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly InMemoryCacheService _cache;
        private readonly ChangeEmailSendCodeCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public ChangeEmailSendCodeCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);
            _cache = new InMemoryCacheService();

            _handler = new ChangeEmailSendCodeCommandHandler(_repo, _uow, _cache);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldGenerateCodeAndSaveToCache()
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

            var command = new ChangeEmailSendCodeCommand(user.Id, "new@example.com");

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();

            var cacheKey = $"ConfirmCode for {user.Login.Value.ToLowerInvariant()}";
            var cachedCode = await _cache.GetStringAsync(cacheKey);
            cachedCode.Should().NotBeNullOrEmpty();

            var outboxMessages = await _context.Set<OutboxMessage>()
                .AsNoTracking()
                .ToListAsync(_ct);

            outboxMessages.Should().ContainSingle();
        }

        [Fact]
        public async Task Handle_EmailAlreadyTaken_ShouldReturnConflict()
        {
            var existingUser = User.Create(
                Login.Create("existing"),
                UserName.Create("Existing"),
                Email.Create("taken@example.com"),
                statusId: Guid.NewGuid(),
                genderId: Guid.NewGuid(),
                countryId: Guid.NewGuid());

            var otherUser = User.Create(
                Login.Create("other"),
                UserName.Create("Other"),
                Email.Create("other@example.com"),
                statusId: Guid.NewGuid(),
                genderId: Guid.NewGuid(),
                countryId: Guid.NewGuid());

            await _context.Users.AddRangeAsync(existingUser, otherUser);
            await _context.SaveChangesAsync(_ct);

            var command = new ChangeEmailSendCodeCommand(otherUser.Id, "taken@example.com");

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.Conflict)?.Code.Should().Be(ErrorCode.Conflict);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFound()
        {
            var command = new ChangeEmailSendCodeCommand(Guid.NewGuid(), "any@example.com");
            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}