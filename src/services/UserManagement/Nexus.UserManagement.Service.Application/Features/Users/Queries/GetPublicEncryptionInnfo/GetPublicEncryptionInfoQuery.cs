using MediatR;
using Shared.Contracts.UserManagement.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed record GetPublicEncryptionInfoQuery(string Login) : IRequest<Result<PublicEncryptionInfoResponse>>;
}