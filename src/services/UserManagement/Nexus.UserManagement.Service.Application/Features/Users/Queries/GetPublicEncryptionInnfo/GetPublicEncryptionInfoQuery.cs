using MediatR;
using Shared.Contracts.UserMenagement.Responses;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetPublicEncryptionInnfo
{
    public sealed record GetPublicEncryptionInfoQuery(string Login) : IRequest<Result<PublicEncryptionInfoResponse>>;
}