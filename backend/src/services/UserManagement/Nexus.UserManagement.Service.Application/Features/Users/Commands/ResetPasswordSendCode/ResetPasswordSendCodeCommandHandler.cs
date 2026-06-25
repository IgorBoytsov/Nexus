using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Crossdyne.Toolkit.Primitives;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Contracts.Cache.Interfaces;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode
{
    public sealed class ResetPasswordSendCodeCommandHandler(
        IUserRepository userRepository,
        IRedisCacheService redis) : IRequestHandler<ResetPasswordSendCodeCommand, Result>
    {
        public async Task<Result> Handle(ResetPasswordSendCodeCommand request, CancellationToken cancellationToken)
        {
            Maybe<User> maybeUser = await userRepository.GetByAsync(u => u.Login == request.Login);

            if (maybeUser.IsNone)
                return Result.Failure(new Error(ErrorCode.NotFound, "Пользователя с таким логином не существует."));

            User user = maybeUser.Value;

            var code = GenerateCode();

            var redisResult = await redis.SetStringAsync($"ConfirmCode for {request.Login}", code, TimeSpan.FromMinutes(5));

            if (!redisResult)
                return Result.Failure(new Error(ErrorCode.Server, "Произошла ошибка на стороне сервера"));

                return Result.Success();
        }

        private string GenerateCode()
        {
            var rnd = new Random();
            List<int> number = [];

            for (int i = 0; i < 6; i++)
            {
                number.Add(rnd.Next(0,9));
            }

            return string.Join("", number);
        }
    }
}