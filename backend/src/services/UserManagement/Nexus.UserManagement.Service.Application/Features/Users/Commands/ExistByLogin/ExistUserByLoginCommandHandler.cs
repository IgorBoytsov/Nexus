using Crossdyne.Toolkit.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ExistByLogin
{
    public sealed class ExistUserByLoginCommandHandler(IWriteDbContext context) : IRequestHandler<ExistUserByLoginCommand, Result>
    {
        private readonly IWriteDbContext _context = context;

        public async Task<Result> Handle(ExistUserByLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exist = await _context.Users.AnyAsync(x => x.Login == request.Login);

                if (!exist)
                    return Result.Failure(new Error(ErrorCode.NotExists, "Пользователь не найден"));

                return Result.Success();
            }
            catch (System.Exception)
            {
                return Result.Failure(new Error(ErrorCode.Server, "Ошибка на стороне сервера"));
            }
        }
    }
}