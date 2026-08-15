using System.Text.Json;
using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nexus.Authentication.Service.Application.Extensions;
using Nexus.Authentication.Service.Application.Features.Commands.Refresh;
using Nexus.Authentication.Service.Application.Interfaces.HttpClients;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Nexus.Authentication.Service.Infrastructure.HttpClients;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using Nexus.Authentication.Service.Infrastructure.Persistence.Repositories.AccessDatas;
using Shared.Contracts.UserManagement.Responses;
using Shared.Test.Cache;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Nexus.Authentication.Service.Integration.Tests.Handlers.Commands
{
    public class RefreshTokenCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly AuthenticationContext _context;
        private readonly IAccessDataRepository _accessDataRepo;
        private readonly IUnitOfWork _uow;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserManagementServiceClient _userManagementServiceClient;
        private readonly IDistributedLockProvider _lockProvider;
        private readonly RefreshTokenCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public RefreshTokenCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _accessDataRepo = new AccessDataRepository(_context);
            _uow = fixture.CreateUnitOfWork(_context);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:Secret"] = "super-secret-key-for-testing-only-32bytes!",
                    ["JwtSettings:Issuer"] = "TestIssuer",
                    ["JwtSettings:Audience"] = "TestAudience",
                    ["JwtSettings:ExpiryMinutes"] = "60"
                })
                .Build();
            _jwtTokenGenerator = new JwtTokenGenerator(config);

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(fixture.UserManagementServiceMock.Urls[0])
            };
            var jsonOptions = Options.Create(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            _userManagementServiceClient = new UserManagementServiceClient(httpClient, jsonOptions);

            _lockProvider = new InMemoryDistributedLockProvider();

            _handler = new RefreshTokenCommandHandler(
                _uow,
                _accessDataRepo,
                _jwtTokenGenerator,
                _userManagementServiceClient,
                _lockProvider,
                NullLogger<RefreshTokenCommandHandler>.Instance);
        }

        public async ValueTask InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _fixture.UserManagementServiceMock.Reset();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        private void SetupUserManagementServiceMock(Guid userId, UserAuthDataResponse response)
        {
            _fixture.UserManagementServiceMock
                .Given(Request.Create()
                    .WithPath($"/internal/api/users/by-id/{userId}")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })));
        }

        private void SetupUserManagementServiceMockNotFound(Guid userId)
        {
            _fixture.UserManagementServiceMock
                .Given(Request.Create()
                    .WithPath($"/internal/api/users/by-id/{userId}")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(404));
        }

        [Fact]
        public async Task Handle_ValidRefreshToken_ShouldReturnNewTokensAndReplaceOldOne()
        {
            var userId = Guid.NewGuid();
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenHash = TokenHasher.Hash(refreshToken);

            var accessData = AccessData.Create(
                userId: userId,
                refreshTokenHash: refreshTokenHash,
                creationDate: DateTime.UtcNow,
                expiryDate: DateTime.UtcNow.AddDays(30),
                isUsed: false,
                isRevoked: false);

            await _context.AccessData.AddAsync(accessData, _ct);
            await _context.SaveChangesAsync(_ct);

            var userResponse = new UserAuthDataResponse(
                Id: userId.ToString(),
                Login: "testuser",
                EncryptedDek: "dek",
                DekVersion: 1,
                EncryptedVerifier: "verifier",
                ClientSalt: "salt",
                SrpVersion: 1,
                SrpCryptoVersion: 1,
                EncryptedVerifierWrapKey: "wrap",
                KeyWrapVersion: 1,
                AsymmetricKeyId: "keyid",
                Roles: new List<string> { "User" });

            SetupUserManagementServiceMock(userId, userResponse);

            var command = new RefreshTokenCommand(refreshToken);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.AccessToken.Should().NotBeNullOrEmpty();
            result.Value!.RefreshToken.Should().NotBeNullOrEmpty();

            var oldToken = await _context.AccessData
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.RefreshTokenHash == refreshTokenHash, _ct);
            oldToken.Should().BeNull();

            var newTokens = await _context.AccessData.AsNoTracking().ToListAsync(_ct);
            newTokens.Should().HaveCount(1);
            newTokens[0].UserId.Should().Be(userId);
            newTokens[0].IsUsed.Should().BeFalse();
            newTokens[0].IsRevoked.Should().BeFalse();
            newTokens[0].RefreshTokenHash.Should().NotBe(refreshTokenHash);
        }

        [Fact]
        public async Task Handle_RefreshTokenNotFound_ShouldReturnUnauthorized()
        {
            var command = new RefreshTokenCommand("non-existing-token");

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.Unauthorized)?.Code.Should().Be(ErrorCode.Unauthorized);
        }

        [Fact]
        public async Task Handle_RefreshTokenAlreadyUsed_ShouldReturnUnauthorized()
        {
            var userId = Guid.NewGuid();
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var hash = TokenHasher.Hash(refreshToken);

            var accessData = AccessData.Create(userId, hash, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), isUsed: true, isRevoked: false);

            await _context.AccessData.AddAsync(accessData, _ct);
            await _context.SaveChangesAsync(_ct);

            var result = await _handler.Handle(new RefreshTokenCommand(refreshToken), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.Unauthorized)?.Code.Should().Be(ErrorCode.Unauthorized);
        }

        [Fact]
        public async Task Handle_RefreshTokenRevoked_ShouldReturnUnauthorized()
        {
            var userId = Guid.NewGuid();
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var hash = TokenHasher.Hash(refreshToken);

            var accessData = AccessData.Create(userId, hash, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), isUsed: false, isRevoked: true);

            await _context.AccessData.AddAsync(accessData, _ct);
            await _context.SaveChangesAsync(_ct);

            var result = await _handler.Handle(new RefreshTokenCommand(refreshToken), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.Unauthorized)?.Code.Should().Be(ErrorCode.Unauthorized);
        }

        [Fact]
        public async Task Handle_RefreshTokenExpired_ShouldReturnUnauthorized()
        {
            var userId = Guid.NewGuid();
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var hash = TokenHasher.Hash(refreshToken);

            var accessData = AccessData.Create(userId, hash, DateTime.UtcNow.AddDays(-60), DateTime.UtcNow.AddDays(-1), isUsed: false, isRevoked: false);

            await _context.AccessData.AddAsync(accessData, _ct);
            await _context.SaveChangesAsync(_ct);

            var result = await _handler.Handle(new RefreshTokenCommand(refreshToken), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.Unauthorized)?.Code.Should().Be(ErrorCode.Unauthorized);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFound()
        {
            var userId = Guid.NewGuid();
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var hash = TokenHasher.Hash(refreshToken);

            var accessData = AccessData.Create(userId, hash, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), isUsed: false, isRevoked: false);

            await _context.AccessData.AddAsync(accessData, _ct);
            await _context.SaveChangesAsync(_ct);

            SetupUserManagementServiceMockNotFound(userId);

            var result = await _handler.Handle(new RefreshTokenCommand(refreshToken), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}