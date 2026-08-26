using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.RecoveryViaKeys;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class RecoveryViaKeysCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly RecoveryViaKeysCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public RecoveryViaKeysCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);

            _handler = new RecoveryViaKeysCommandHandler(_uow, _repo);
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

            user.AddEmailAuthenticator(user.Email);
            user.AddSrpAuthenticator(user.Login, Verificator.Create("verificator"), Salt.Create("salt"), SrpVersion.Create(1), CredentialBlob.Create("blob"), CryptoVersion.Create(1), AsymmetricKeyId.Create("v1"), CryptoVersion.Create(1));
            user.AddMainDek(EncryptedValue.Create("value"), Salt.Create("salt"), CryptoVersion.Create(1));

            await _context.Users.AddAsync(user, _ct);
            await _context.SaveChangesAsync(_ct);

            var command = new RecoveryViaKeysCommand(user.Login, "NewEncryptedVerifier", "NewSrpSalt", 2, 2, "NewVerifierWrapKey", 2, "v1", "NewEncryptedDek", "NewDekSalt", 2, [ new RecoveryKeyCommandData("value1", 2), new RecoveryKeyCommandData("value2", 2)]);
            var result = await _handler.Handle(command, _ct);

            var userInDb = await _context.Users.AsNoTracking().Include(u => u.UserAuthenticators).Include(u => u.Deks).Include(u => u.RecoveryKeys).FirstOrDefaultAsync(u => u.Id == user.Id, _ct);

            result.IsSuccess.Should().BeTrue();
            userInDb?.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault()?.EncryptedVerifier?.Value.Should().Be("NewEncryptedVerifier");
            userInDb?.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault()?.Salt?.Value.Should().Be("NewSrpSalt");
            userInDb?.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault()?.SrpVersion?.Value.Should().Be(2);
            userInDb?.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault()?.CryptoVersion?.Value.Should().Be(2);
            userInDb?.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault()?.KeyWrapVersion?.Value.Should().Be(2);
            userInDb?.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefault()?.EncryptedVerifierWrapKey?.Value.Should().Be("NewVerifierWrapKey");
            userInDb?.Deks.FirstOrDefault()?.Salt.Value.Should().Be("NewDekSalt");
            userInDb?.Deks.FirstOrDefault()?.Version.Value.Should().Be(2);
            user.RecoveryKeys.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFound()
        {
            var command = new RecoveryViaKeysCommand("Login", "NewEncryptedVerifier", "NewSrpSalt", 2, 2, "NewVerifierWrapKey", 2, "v1", "NewEncryptedDek", "NewDekSalt", 2, [ new RecoveryKeyCommandData("value", 2), new RecoveryKeyCommandData("value", 2)]);
            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}