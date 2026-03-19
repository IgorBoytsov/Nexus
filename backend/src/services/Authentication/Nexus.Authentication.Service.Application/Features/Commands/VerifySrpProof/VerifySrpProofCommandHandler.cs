using MediatR;
using Nexus.Authentication.Service.Application.Common.Abstractions;
using Nexus.Authentication.Service.Application.Services;
using Nexus.Authentication.Service.Domain.Models;
using Quantropic.Security.Abstractions;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;
using Shared.Kernel.Errors;

namespace Nexus.Authentication.Service.Application.Features.Commands.VerifySrpProof
{
    public class VerifySrpProofHandler(
       IApplicationDbContext context,
       IRedisCacheService redisCacheService,
       IJwtTokenGenerator jwtTokenGenerator,
       ISrpServer srpServer,
       IUserManagementServiceClient userManagementClient) : IRequestHandler<VerifySrpProofCommand, Result<AuthResponse>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IRedisCacheService _redisCacheService = redisCacheService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly ISrpServer _srpServer = srpServer;
        private readonly IUserManagementServiceClient _userManagementServiceClient = userManagementClient;

        public async Task<Result<AuthResponse>> Handle(VerifySrpProofCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.A) || string.IsNullOrWhiteSpace(request.M1))
                return Result<AuthResponse>.Failure(new Error(AppErrors.Validation, "Параметры SRP не могут быть пустыми"));

            var session = await _redisCacheService.GetJsonAsync<SrpSessionState>($"srp_{request.Login}");

            if (session is null)
                return Result<AuthResponse>.Failure(new Error(ErrorCode.NotFound, "Сессия просрочена"));

            var M2_server = _srpServer.VerifySrpProof(session, request.A, request.M1);

            var userData = await _userManagementServiceClient.GetUserByLoginAsync(request.Login);
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(userData!);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var accessData = AccessData.Create(Guid.Parse(userData!.Id), refreshToken, accessToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30),
                isUsed: false,
                isRevoked: false);

            await _context.AccessData.AddAsync(accessData, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _redisCacheService.RemoveAsync($"srp_{request.Login}");
            return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, M2_server));
        }
    }
}