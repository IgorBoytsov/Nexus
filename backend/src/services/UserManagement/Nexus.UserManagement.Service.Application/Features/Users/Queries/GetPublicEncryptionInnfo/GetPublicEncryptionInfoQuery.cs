using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Contracts.UserManagement.Responses;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed record GetPublicEncryptionInfoQuery(string Login) : IRequest<Result<PublicEncryptionInfoResponse>>;
}