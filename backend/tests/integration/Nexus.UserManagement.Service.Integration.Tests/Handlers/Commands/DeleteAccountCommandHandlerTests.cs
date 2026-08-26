using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.Delete;
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
    public class DeleteAccountCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly DeleteAccountCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public DeleteAccountCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);

            _handler = new DeleteAccountCommandHandler(_repo, _uow);
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

            var userInDb = await _context.Users.AsNoTracking().Include(u => u.UserAuthenticators).Include(u => u.Deks).FirstOrDefaultAsync(u => u.Id == user.Id, _ct);

            userInDb?.UserAuthenticators.Should().HaveCount(2);
            userInDb?.Deks.Should().HaveCount(1);

            var command = new DeleteAccountCommand(user.Id);
            var result = await _handler.Handle(command, _ct);

            var userBeforeDeleted = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id, _ct);
            var userAuthenticators = await _context.UserAuthenticators.AsNoTracking().Where(ua => ua.UserId == user.Id).CountAsync(_ct);
            var userDeks = await _context.Deks.AsNoTracking().Where(ua => ua.UserId == user.Id).CountAsync(_ct);

            result.IsSuccess.Should().BeTrue();
            userAuthenticators.Should().Be(0);
            userDeks.Should().Be(0);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFound()
        {
            var command = new DeleteAccountCommand(Guid.NewGuid());
            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}