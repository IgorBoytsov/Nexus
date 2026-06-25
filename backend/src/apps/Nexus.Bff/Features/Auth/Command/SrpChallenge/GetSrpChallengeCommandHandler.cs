using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.Authentication.Responses;
using Shared.Contracts.Authentication.Requests;

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