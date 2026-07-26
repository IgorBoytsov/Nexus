using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Common;
using Nexus.Bff.Services;
using System.Security.Cryptography;
using Nexus.Bff.Extensions;
using Shared.Contracts.Cache.Interfaces;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Authentication.Responses;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public sealed class VerifySrpProofCommandHandler(
        IAuthClient authClient, 
        ICacheService cache,
        IJwtReadService jwtReader) : IRequestHandler<VerifySrpProofCommand, Result<VerifierSrpProofDTO>>
    {   
        private readonly IAuthClient _authClient = authClient;

        public async Task<Result<VerifierSrpProofDTO>> Handle(VerifySrpProofCommand request, CancellationToken cancellationToken)
        {
            var result = await _authClient.VerifierSrpProof(new SrpVerifyRequest(request.Login.ToLowerInvariant(), request.A, request.M1));

            if (result.IsFailure)
                return Result<VerifierSrpProofDTO>.Failure(result.Errors);

            AuthResponse authData = result.Value!;

            var data = jwtReader.ExtractData(authData.AccessToken);
            var sessionId = Guid.NewGuid().ToString();

            var userSession = new UserSession(sessionId, authData.AccessToken, authData.RefreshToken, data.ExpiredTime, data.UserId, data.Login.ToLowerInvariant());

            Span<byte> tempAuthTokenBytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(tempAuthTokenBytes);
            string tempAuthToken = Convert.ToBase64String(tempAuthTokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

            var resultCache = await cache.SetJsonAsync(RedisKeyExtensions.SrpTempToken(tempAuthToken), userSession, TimeSpan.FromMinutes(2));

            if (!resultCache)
                return new Error(ErrorCode.Server, "Произошла непредвиденная ошибка на стороне сервера");

            return new VerifierSrpProofDTO(authData.M2!, tempAuthToken);
        }
    }
}