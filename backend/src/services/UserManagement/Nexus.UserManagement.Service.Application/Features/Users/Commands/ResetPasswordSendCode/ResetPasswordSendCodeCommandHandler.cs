using MediatR;
using Crossdyne.Toolkit.Results;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Abstractions.Cache;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode
{
    public sealed class ResetPasswordSendCodeCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICacheService redis) : IRequestHandler<ResetPasswordSendCodeCommand, Result>
    {
        public async Task<Result> Handle(ResetPasswordSendCodeCommand request, CancellationToken cancellationToken)
        {
            string normalizeLogin = request.Login.ToLowerInvariant();

            Maybe<User> maybeUser = await userRepository.GetByAsync(u => u.Login == normalizeLogin);

            if (maybeUser.IsNone)
                return Result.Failure(new Error(ErrorCode.NotFound, "Пользователя с таким логином не существует."));

            User user = maybeUser.Value;

            var code = user.GetResetPasswordCode();

            var redisResult = await redis.SetStringAsync($"ConfirmCode for {normalizeLogin}", code, TimeSpan.FromMinutes(10));

            if (!redisResult)
                return Result.Failure(new Error(ErrorCode.Server, "Произошла ошибка на стороне сервера"));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}