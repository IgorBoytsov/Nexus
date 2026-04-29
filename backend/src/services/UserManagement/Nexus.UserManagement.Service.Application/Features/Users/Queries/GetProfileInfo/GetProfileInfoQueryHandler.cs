using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Quantropic.Toolkit.Results;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo
{
    public sealed class GetProfileInfoQueryHandler(IWriteDbContext context) : IRequestHandler<GetProfileInfoQuery, Result<ProfileInfoResponse>>
    {
        private readonly IWriteDbContext _context = context;

        public async Task<Result<ProfileInfoResponse>> Handle(GetProfileInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            return Result<ProfileInfoResponse>.Success(new ProfileInfoResponse(user!.Login, user!.Email, user.Phone?.Value));
        }
    }
}