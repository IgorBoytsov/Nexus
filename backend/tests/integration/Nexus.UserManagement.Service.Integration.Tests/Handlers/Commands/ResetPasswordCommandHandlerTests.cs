using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPassword;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.SmartEnums;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserAuthenticator;
using Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands;

public class ResetPasswordCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private readonly UserManagementContext _context;
    private readonly UserRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly OutboxSignal _outboxSignal;
    private readonly ResetPasswordCommandHandler _handler;
    private readonly CancellationToken _ct = default;

    public ResetPasswordCommandHandlerTests(TestFixture fixture)
    {
        _fixture = fixture;

        _context = fixture.CreateDbContext();
        _repo = new UserRepository(_context);
        _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
        _uow = fixture.CreateUnitOfWork(_context);

        _handler = new ResetPasswordCommandHandler(_uow, _repo);
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
    public async Task Handle_ExistingUser_ShouldResetPasswordAndReplaceRecoveryKeys()
    {
        var login = Login.Create("testuser");
        var user = User.Create(
            login,
            UserName.Create("Test User"),
            Email.Create("test@example.com"),
            statusId: EnumStatus.Active.Id,
            genderId: null,
            countryId: null);

        user.AddMainDek(
            EncryptedValue.Create(FakeBase64()),
            Salt.Create(FakeBase64(16)),
            CryptoVersion.Create(1));

        user.AddSrpAuthenticator(
            login,
            Verificator.Create(FakeBase64()),
            Salt.Create(FakeBase64(16)),
            SrpVersion.Create(1),
            CredentialBlob.Create(FakeBase64()),
            CryptoVersion.Create(1),
            AsymmetricKeyId.Create(Guid.NewGuid().ToString()),
            CryptoVersion.Create(1));

        user.AddRecoveryKey(EncryptedValue.Create(FakeBase64()), CryptoVersion.Create(1), KeyHint.Create("old1"));
        user.AddRecoveryKey(EncryptedValue.Create(FakeBase64()), CryptoVersion.Create(1), KeyHint.Create("old2"));

        await _context.Users.AddAsync(user, _ct);
        await _context.SaveChangesAsync(_ct);

        var command = new ResetPasswordCommand(
            Login: "testuser",
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

        var updated = await _context.Users
            .AsNoTracking()
            .Include(u => u.Deks)
            .Include(u => u.UserAuthenticators)
            .Include(u => u.RecoveryKeys)
            .FirstOrDefaultAsync(u => u.Id == user.Id, _ct);

        updated.Should().NotBeNull();
        updated!.RecoveryKeys.Should().HaveCount(3);
        updated.Deks.Should().ContainSingle();
        updated.UserAuthenticators.Should().ContainSingle();

        var outbox = await _context.Set<OutboxMessage>()
            .AsNoTracking()
            .Where(o => o.EventType.Contains("UserPasswordReset"))
            .ToListAsync(_ct);

        outbox.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_NonExistingUser_ShouldReturnFailure()
    {
        var command = new ResetPasswordCommand(
            Login: "ghost_user",
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
            RecoveryKeys: [new RecoveryKeyCommandData(FakeBase64(), 1)]);

        var result = await _handler.Handle(command, _ct);

        result.IsSuccess.Should().BeFalse();
    }
}