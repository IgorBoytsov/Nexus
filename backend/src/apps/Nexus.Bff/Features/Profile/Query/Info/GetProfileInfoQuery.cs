using MediatR;
using Quantropic.Toolkit.Results;

namespace Nexus.Bff.Features.Profile.Query.Info
{
    public sealed record GetProfileInfoQuery(string UserId) : IRequest<Result<ProfileInfoResponse>>;
}