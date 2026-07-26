using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Extensions;
using Nexus.Bff.Infrastructure.Clients;
using Shared.Contracts.Authentication.Requests;
using Shared.Contracts.Cache.Interfaces;
using Shared.Contracts.Common;
using Unit = Crossdyne.Toolkit.Primitives.Unit;

namespace Nexus.Bff.Features.Auth.Command.Logout
{
    public sealed class LogoutCommandHandler(IAuthClient client, IRedisCacheService cache) : IRequestHandler<LogoutCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {       
            var cacheSessionKey = RedisKeyExtensions.SessionKey(request.SessionId);

            var userSession = await cache.GetJsonAsync<UserSession>(cacheSessionKey);

            if (userSession is null)
                return Unit.Value;

            await client.Logout(new LogoutRequest(userSession!.RefreshToken));   
            await cache.RemoveAsync(cacheSessionKey);
            await cache.SetRemoveAsync(RedisKeyExtensions.UserSessionsKey(userSession.UserId), request.SessionId);

            return Unit.Value;
        }
    }
}