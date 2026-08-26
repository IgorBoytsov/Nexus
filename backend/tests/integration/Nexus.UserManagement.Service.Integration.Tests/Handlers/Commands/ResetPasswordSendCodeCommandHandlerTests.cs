using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.SmartEnums;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Shared.Test.Cache;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class ResetPasswordSendCodeCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly InMemoryCacheService _cache;
        private readonly ResetPasswordSendCodeCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public ResetPasswordSendCodeCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _cache = new InMemoryCacheService();
            _uow = fixture.CreateUnitOfWork(_context);

            _handler = new ResetPasswordSendCodeCommandHandler(_repo, _uow, _cache);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ExistingUser_ShouldSaveCodeToCacheAndCreateOutboxMessage()
        {
            var login = Login.Create("testuser_2024");
            var user = User.Create(
                login,
                UserName.Create("Test User"),
                Email.Create("test@example.com"),
                statusId: EnumStatus.Active.Id,
                genderId: null,
                countryId: null);

            await _context.Users.AddAsync(user, _ct);
            await _context.SaveChangesAsync(_ct);

            var command = new ResetPasswordSendCodeCommand(login.Value);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();

            var cachedCode = await _cache.GetStringAsync($"ConfirmCode for {login.Value.ToLowerInvariant()}");
            cachedCode.Should().NotBeNullOrEmpty();
            cachedCode!.Length.Should().Be(6);

            var outboxMessages = await _context.Set<OutboxMessage>()
                .AsNoTracking()
                .ToListAsync(_ct);

            outboxMessages.Should().ContainSingle();
            outboxMessages[0].EventType.Should().Contain("PasswordResetRequested");
        }

        [Fact]
        public async Task Handle_NonExistingUser_ShouldReturnNotFound()
        {
            var command = new ResetPasswordSendCodeCommand("ghost_user");
            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.First().Code.Should().Be(ErrorCode.NotFound);

            var cachedCode = await _cache.GetStringAsync("ConfirmCode for ghost_user");
            cachedCode.Should().BeNull();
        }
    }
}