using System.Security.Cryptography;
using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.Register;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class RegisterCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly RegisterCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public RegisterCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);

            _handler = new RegisterCommandHandler(_uow, _repo);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

         private static string FakeBase64(int byteLength = 32) =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength));

        [Fact]
        public async Task Handle_ValidCommand_ShouldRegisterUserWithAllData()
        {
            var genderId = Guid.NewGuid();
            var countryId = Guid.NewGuid();

            await _context.Genders.AddAsync(Gender.Create("Муж"), _ct);
            await _context.Countries.AddAsync(Country.Create("Россия"), _ct);
            await _context.Roles.AddAsync(Role.Create("Admin"), _ct);

            await _context.SaveChangesAsync(_ct);

            var gender = await _context.Genders.FirstOrDefaultAsync(_ct);
            var country = await _context.Countries.FirstOrDefaultAsync(_ct);
            var role = await _context.Roles.FirstOrDefaultAsync(_ct);

            var command = new RegisterCommand(
                Login: "ivan_petrov_2026",
                UserName: "Иван Петров",
                Email: "ivan.petrov@example.com",
                IdGender: gender?.Id,
                IdCountry: country?.Id,
                EncryptedVerifier: FakeBase64(),
                SrpSalt: FakeBase64(16),
                SrpVersion: 1,
                SrpCryptoVersion: 1,
                EncryptedVerifierWrapKey: FakeBase64(),
                KeyWrapVersion: 1,
                AsymmetricKeyId: Guid.NewGuid().ToString(),
                EncryptedDek: FakeBase64(),
                DekSalt: FakeBase64(16),
                CryptoVersion: 1,
                RecoveryKeys:
                [
                    new RecoveryKeyCommandData(EncryptedValue: FakeBase64(), CryptoVersion: 1),
                    new RecoveryKeyCommandData(EncryptedValue: FakeBase64(), CryptoVersion: 1),
                    new RecoveryKeyCommandData(EncryptedValue: FakeBase64(), CryptoVersion: 1)
                ]);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Deks)
                .Include(u => u.UserAuthenticators)
                .Include(u => u.RecoveryKeys)
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Email == command.Email, _ct);

            user.Should().NotBeNull();
            user!.Login.Value.Should().Be(command.Login.ToLowerInvariant());
            user.UserName.Value.Should().Be(command.UserName);
            user.Deks.Should().ContainSingle();
            user.UserAuthenticators.Should().HaveCount(2);
            user.RecoveryKeys.Should().HaveCount(3);
            user.UserRoles.Should().ContainSingle();
        }

        [Fact]
        public async Task Handle_DuplicateEmail_ShouldReturnConflict()
        {
            var existing = User.Create(
                Login.Create("existing_user"),
                UserName.Create("Existing"),
                Email.Create("duplicate@example.com"),
                statusId: Guid.NewGuid(),
                genderId: null,
                countryId: null);

            await _context.Users.AddAsync(existing, _ct);
            await _context.SaveChangesAsync(_ct);

            var command = new RegisterCommand(
                Login: "new_user",
                UserName: "New User",
                Email: "duplicate@example.com",
                IdGender: null,
                IdCountry: null,
                EncryptedVerifier: FakeBase64(),
                SrpSalt: FakeBase64(),
                SrpVersion: 1,
                SrpCryptoVersion: 1,
                EncryptedVerifierWrapKey: FakeBase64(),
                KeyWrapVersion: 1,
                AsymmetricKeyId: Guid.NewGuid().ToString(),
                EncryptedDek: FakeBase64(),
                DekSalt: FakeBase64(),
                CryptoVersion: 1,
                RecoveryKeys: [new RecoveryKeyCommandData(FakeBase64(), 1)]);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}