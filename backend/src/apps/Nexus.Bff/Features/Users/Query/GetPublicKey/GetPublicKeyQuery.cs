using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.Bff.Features.Users.Query.GetPublicKey
{
    public sealed record GetPublicKeyQuery() : IRequest<Result<string>>;
}