using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;

namespace Nexus.Bff.Features.Auth.Command.SrpChallenge
{
    public sealed class GetSrpChallengeCommandHandler(IAuthClient authClient) : IRequestHandler<GetSrpChallengeCommand, Result<SrpChallengeResponse?>>
    {
        private readonly IAuthClient _authClient = authClient;
        
        public async Task<Result<SrpChallengeResponse?>> Handle(GetSrpChallengeCommand request, CancellationToken cancellationToken)
        {
            var result = await _authClient.GetSrpChallenge(new SrpChallengeRequest(request.Login));

            return result;
        }
    }
}