using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Quantropic.Toolkit.Results;
using Shared.Contracts;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode
{
    public sealed class ResetPasswordSendCodeCommandHandler(
        IWriteDbContext context,
        IRedisCacheService redis) : IRequestHandler<ResetPasswordSendCodeCommand, Result>
    {
        private readonly IWriteDbContext _context = context;
        private readonly IRedisCacheService _redis = redis;

        public async Task<Result> Handle(ResetPasswordSendCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user is null)
                return Result.Failure(new Error(ErrorCode.NotFound, "Пользователя с таким логином не существует."));

            var code = GenerateCode();

            var redisResult = await _redis.SetStringAsync($"ConfirmCode for {request.Login}", code, TimeSpan.FromMinutes(5));

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