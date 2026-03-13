using MediatR;
using Quantropic.Toolkit.Results;
using Rebout.Nexus.Contracts.UserManagement.V1;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed record GetPublicEncryptionInfoQuery(string Login) : IRequest<Result<PublicEncryptionInfoResponse>>;
}