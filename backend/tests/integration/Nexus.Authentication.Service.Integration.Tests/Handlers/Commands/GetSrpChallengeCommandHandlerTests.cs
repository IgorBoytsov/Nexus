using System.Text.Json;
using Crossdyne.Security.Abstractions;
using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nexus.Authentication.Service.Application.Extensions;
using Nexus.Authentication.Service.Application.Features.Commands.SrpChallenge;
using Nexus.Authentication.Service.Infrastructure.HttpClients;
using Shared.Contracts.Cache.Abstractions;
using Shared.Contracts.UserManagement.Responses;
using Shared.Test.Cache;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Nexus.Authentication.Service.Integration.Tests.Handlers.Commands
{
    public class GetSrpChallengeCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly ICacheService _cacheService;
        private readonly GetSrpChallengeCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public GetSrpChallengeCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;
            _cacheService = new InMemoryCacheService();

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

            _handler = new GetSrpChallengeCommandHandler(
                userManagementClient,
                _cacheService,
                new FakeSrpServer(),
                new FakeCryptoService(),
                new FakeDataProtector(),
                NullLogger<GetSrpChallengeCommandHandler>.Instance);
        }

        public async ValueTask InitializeAsync()
        {
            _fixture.UserManagementServiceMock.Reset();
            await Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => default;

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

        [Fact]
        public async Task Handle_ExistingUser_ShouldReturnSrpChallengeAndStoreSessionInCache()
        {
            var login = "testuser";
            var normalizedLogin = login.ToLowerInvariant();
            var userId = Guid.NewGuid();

            var userData = new UserAuthDataResponse(
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

            SetupUserByLoginMock(normalizedLogin, userData);

            var command = new GetSrpChallengeCommand(login);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Salt.Should().Be(userData.ClientSalt);
            result.Value!.B.Should().NotBeNullOrEmpty();
            result.Value!.SrpVersion.Should().Be(userData.SrpVersion);
            result.Value!.SrpCryptoVersion.Should().Be(userData.SrpCryptoVersion);

            var cacheKey = RedisKeyExtensions.SrpSession(normalizedLogin);
            var cachedSession = await _cacheService.GetJsonAsync<SrpSessionState>(cacheKey);

            cachedSession.Should().NotBeNull();
            cachedSession!.Login.Should().Be(normalizedLogin);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNotFound()
        {
            var login = "nonexistent";
            SetupUserByLoginMock(login.ToLowerInvariant(), null);

            var result = await _handler.Handle(new GetSrpChallengeCommand(login), _ct);

            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)
                ?.Code.Should().Be(ErrorCode.NotFound);
        }
    }
}