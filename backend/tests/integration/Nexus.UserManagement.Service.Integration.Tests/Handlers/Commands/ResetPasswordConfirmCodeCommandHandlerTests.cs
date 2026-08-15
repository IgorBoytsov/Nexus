using FluentAssertions;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.Kernel.Errors;
using Shared.Test.Cache;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class ResetPasswordConfirmCodeCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly OutboxSignal _outboxSignal;
        private readonly InMemoryCacheService _cache;
        private readonly ResetPasswordConfirmCodeCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public ResetPasswordConfirmCodeCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;
            _context = fixture.CreateDbContext();
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _cache = new InMemoryCacheService();

            _handler = new ResetPasswordConfirmCodeCommandHandler(_cache);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ValidCode_ShouldReturnSuccess_And_RemoveCodeFromCache()
        {
            var login = "testuser";
            var code = "123456";
            await _cache.SetStringAsync($"ConfirmCode for {login.ToLowerInvariant()}", code, TimeSpan.FromMinutes(10));

            var command = new ResetPasswordConfirmCodeCommand(login, code);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();

            var cached = await _cache.GetStringAsync($"ConfirmCode for {login.ToLowerInvariant()}");
            cached.Should().BeNull();
        }

        [Fact]
        public async Task Handle_CodeNotFound_ShouldReturnTimeEndedError()
        {
            var command = new ResetPasswordConfirmCodeCommand("unknown_user", "123456");

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.First().Code.Should().Be(AppErrors.TimeEnded);
        }

        [Fact]
        public async Task Handle_WrongCode_ShouldReturnIncorrectValueError()
        {
            var login = "testuser";
            await _cache.SetStringAsync($"ConfirmCode for {login.ToLowerInvariant()}", "111111", TimeSpan.FromMinutes(10));

            var command = new ResetPasswordConfirmCodeCommand(login, "999999");

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.First().Code.Should().Be(AppErrors.IncorrectValue);

            var cached = await _cache.GetStringAsync($"ConfirmCode for {login.ToLowerInvariant()}");
            cached.Should().Be("111111");
        }
    }
}