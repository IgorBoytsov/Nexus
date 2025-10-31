using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.SmartEnums;
using Shared.Kernel.Results;
using Shared.Security.Hasher;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.Register
{
    public sealed class RegisterCommandHandler(IWriteDbContext writeContext, IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IWriteDbContext _writeContext = writeContext;
        private readonly IPasswordHasher _hasher = passwordHasher;

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string passwordHash = _hasher.HashPassword(request.Password);

                var user = User.Create(request.Login, request.UserName, passwordHash, request.Email, request.Phone, EnumStatus.Active.Id, EnumRole.User.Id, request.IdGender, request.IdCountry);

                await _writeContext.Users.AddAsync(user, cancellationToken);
                await _writeContext.SaveChangesAsync(cancellationToken);

                user.ClearDomainEvents();

                return Result.Success();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Result.Failure(new Error(ErrorCode.Conflict, "Email уже занят."));
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Save, "Произошла критическая ошибка на стороне сервера при регистрации"));
            }
        }
    }
}