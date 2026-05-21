using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Enums;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Contracts;
using Nexus.UserManagement.Service.Domain.ValueObjects.Deks;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed class GetUserByIdQueryHandler(IWriteDbContext writeContext) : IRequestHandler<GetUserByIdQuery, Result<UserAuthDataResponse>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<UserAuthDataResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _writeContext.Users
                    .Include(u => u.UserRoles)
                        .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
                    
                if (user == null)
                    return Result<UserAuthDataResponse>.Failure(new Error(ErrorCode.NotFound, "Такого пользователя нету"));

                var roleIds = user.UserRoles.Select(ur => ur.RoleId);
                var roleNames = await _writeContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync(cancellationToken);
                var dek = await _writeContext.Deks.FirstOrDefaultAsync(d => d.Type == DekType.Main);
                var srp = await _writeContext.UserAuthenticators.OfType<SrpAuthenticator>().FirstOrDefaultAsync(ua => ua.UserId == user.Id);

                var userAuth = new UserAuthDataResponse(
                    user.Id.Value.ToString(),
                    user.Login,
                    srp!.EncryptedVerifier!, 
                    srp.Salt!,
                    srp.SrpVersion!.Value.Value,
                    srp.EncryptedVerifierWrapKey!,
                    srp.KeyWrapVersion!.Value.Value,
                    srp.AsymmetricKeyId!,
                    dek!.EncryptedValue,
                    roleNames.Select(rn => rn.Value).ToList());

                return Result<UserAuthDataResponse>.Success(userAuth);
            }
            catch (Exception)
            {
                return Result<UserAuthDataResponse>.Failure(new Error(ErrorCode.Server, "Произошла непредвиденная серверная ошибка"));
            }
        }
    }
}