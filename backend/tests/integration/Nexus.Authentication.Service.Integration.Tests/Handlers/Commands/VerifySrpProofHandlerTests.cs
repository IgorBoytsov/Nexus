using System.Text.Json;
using Crossdyne.Security.Abstractions;
using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nexus.Authentication.Service.Application.Extensions;
using Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof;
using Nexus.Authentication.Service.Application.Interfaces.Repositories;
using Nexus.Authentication.Service.Application.Interfaces.UnitOfWork;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Infrastructure.HttpClients;
using Nexus.Authentication.Service.Infrastructure.Persistence.Contexts;
using Nexus.Authentication.Service.Infrastructure.Persistence.Repositories.AccessDatas;
using Shared.Abstractions.Cache;
using Shared.Contracts.UserManagement.Responses;
using Shared.Kernel.Errors;
using Shared.Test.Cache;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Nexus.Authentication.Service.Integration.Tests.Handlers.Commands
{
    public class VerifySrpProofHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly AuthenticationContext _context;
        private readonly IAccessDataRepository _accessDataRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICacheService _cacheService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly VerifySrpProofHandler _handler;
        private readonly CancellationToken _ct = default;

        public VerifySrpProofHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;
            _context = fixture.CreateDbContext();
            _accessDataRepo = new AccessDataRepository(_context);
            _uow = fixture.CreateUnitOfWork(_context);
            _cacheService = new InMemoryCacheService();

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
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var userManagementClient = new UserManagementServiceClient(httpClient, jsonOptions);

            _handler = new VerifySrpProofHandler(
                _uow,
                _accessDataRepo,
                _cacheService,
                _jwtTokenGenerator,
                new FakeSrpServer(),
                userManagementClient,
                NullLogger<VerifySrpProofHandler>.Instance);
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

        private void SetupUserByLoginMock(string login, UserAuthDataResponse? response)
        {
            if (response is null)
            {
                _fixture.UserManagementServiceMock
                    .Given(Request.Create()
                        .WithPath($"/internal/api/users/by-login/{login}")
                        .UsingGet())
                    .RespondWith(Response.Create().WithStatusCode(404));
                return;
            }

            _fixture.UserManagementServiceMock
                .Given(Request.Create()
                    .WithPath($"/internal/api/users/by-login/{login}")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })));
        }

        private async Task SeedSrpSessionAsync(string login)
        {
            var normalizedLogin = login.ToLowerInvariant();
            var session = new SrpSessionState(
                normalizedLogin,
                privateKeyB: [4, 5, 6],
                verifier: [1, 2, 3],
                publicKeyB: [7, 8, 9],
                salt: [10, 11, 12]);

            var cacheKey = RedisKeyExtensions.SrpSession(normalizedLogin);
            await _cacheService.SetJsonAsync(cacheKey, session, TimeSpan.FromMinutes(2));
        }

        private UserAuthDataResponse CreateUserData(string login, Guid userId) =>
            new(
                Id: userId.ToString(),
                Login: login,
                EncryptedDek: "dek",
                DekVersion: 1,
                EncryptedVerifier: "enc-verifier",
                ClientSalt: Convert.ToBase64String(new byte[] { 10, 11, 12 }),
                SrpVersion: 1,
                SrpCryptoVersion: 2,
                EncryptedVerifierWrapKey: "wrap-key",
                KeyWrapVersion: 1,
                AsymmetricKeyId: "keyid",
                Roles: new List<string> { "User" });

        [Fact]
        public async Task Handle_ValidProof_ShouldReturnAuthResponsePersistTokensAndRemoveSession()
        {
            var login = "testuser";
            var userId = Guid.NewGuid();
            var userData = CreateUserData(login, userId);

            SetupUserByLoginMock(login.ToLowerInvariant(), userData);
            await SeedSrpSessionAsync(login);

            var command = new VerifySrpProofCommand(login, "valid-A", "valid-M1");

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.AccessToken.Should().NotBeNullOrEmpty();
            result.Value!.RefreshToken.Should().NotBeNullOrEmpty();
            result.Value!.M2.Should().NotBeNullOrEmpty();

            var accessDataInDb = await _context.AccessData
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserId == userId, _ct);
            accessDataInDb.Should().NotBeNull();
            accessDataInDb!.IsUsed.Should().BeFalse();
            accessDataInDb.IsRevoked.Should().BeFalse();

            var cacheKey = RedisKeyExtensions.SrpSession(login.ToLowerInvariant());
            var cachedSession = await _cacheService.GetJsonAsync<SrpSessionState>(cacheKey);
            cachedSession.Should().BeNull();
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFound()
        {
            var login = "nonexistent";
            SetupUserByLoginMock(login.ToLowerInvariant(), null);

            var result = await _handler.Handle(new VerifySrpProofCommand(login, "A", "M1"), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)
                ?.Code.Should().Be(ErrorCode.NotFound);
        }

        [Fact]
        public async Task Handle_EmptyA_ShouldReturnValidationError()
        {
            var login = "testuser";
            var userData = CreateUserData(login, Guid.NewGuid());

            SetupUserByLoginMock(login.ToLowerInvariant(), userData);

            var result = await _handler.Handle(new VerifySrpProofCommand(login, "", "M1"), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == AppErrors.Validation)
                ?.Code.Should().Be(AppErrors.Validation);
        }

        [Fact]
        public async Task Handle_EmptyM1_ShouldReturnValidationError()
        {
            var login = "testuser";
            var userData = CreateUserData(login, Guid.NewGuid());

            SetupUserByLoginMock(login.ToLowerInvariant(), userData);

            var result = await _handler.Handle(new VerifySrpProofCommand(login, "A", ""), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == AppErrors.Validation)
                ?.Code.Should().Be(AppErrors.Validation);
        }

        [Fact]
        public async Task Handle_SessionNotFound_ShouldReturnSessionExpired()
        {
            var login = "testuser";
            var userData = CreateUserData(login, Guid.NewGuid());

            SetupUserByLoginMock(login.ToLowerInvariant(), userData);

            var result = await _handler.Handle(new VerifySrpProofCommand(login, "A", "M1"), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == AppErrors.SessionExpired)
                ?.Code.Should().Be(AppErrors.SessionExpired);
        }
    }
}