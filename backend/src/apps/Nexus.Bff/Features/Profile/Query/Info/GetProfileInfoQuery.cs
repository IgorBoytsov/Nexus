using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Profile.Query.Info
{
    public sealed record GetProfileInfoQuery(string UserId) : IRequest<Result<ProfileInfoResponse>>, IHasStringUserId;
}