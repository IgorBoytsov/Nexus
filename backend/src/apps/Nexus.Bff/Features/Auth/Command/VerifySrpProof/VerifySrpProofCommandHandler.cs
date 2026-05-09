using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Crossdyne.Toolkit.Results;
using Rebout.Nexus.Contracts.Authentication.v1;

namespace Nexus.Bff.Features.Auth.Command.VerifySrpProof
{
    public sealed class VerifySrpProofCommandHandler(IAuthClient authClient) : IRequestHandler<VerifySrpProofCommand, Result<AuthResponse?>>
    {   
        private readonly IAuthClient _authClient = authClient;

        public async Task<Result<AuthResponse?>> Handle(VerifySrpProofCommand request, CancellationToken cancellationToken)
            => await _authClient.VerifierSrpProof(new SrpVerifyRequest(request.Login, request.A, request.M1));
    }
}