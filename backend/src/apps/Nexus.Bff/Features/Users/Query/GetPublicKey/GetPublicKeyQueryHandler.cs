using MediatR;
using Nexus.Bff.Infrastructure.Clients;
using Quantropic.Toolkit.Results;

namespace Nexus.Bff.Features.Users.Query.GetPublicKey
{
    public sealed class GetPublicKeyQueryHandler(IAuthClient authClient) : IRequestHandler<GetPublicKeyQuery, Result<string>>
    {
        private readonly IAuthClient _authClient = authClient;

        public async Task<Result<string>> Handle(GetPublicKeyQuery request, CancellationToken cancellationToken)
            => await _authClient.GetPublicKey();
    }
}