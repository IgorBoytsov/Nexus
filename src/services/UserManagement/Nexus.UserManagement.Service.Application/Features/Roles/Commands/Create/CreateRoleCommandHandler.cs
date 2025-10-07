using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Roles.Commands.Create
{
    public sealed class CreateRoleCommandHandler(IWriteDbContext writeContext) : IRequestHandler<CreateRoleCommand, Result<Guid>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = Role.Create(request.Name);

                await _writeContext.Roles.AddAsync(role, cancellationToken);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(role.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}