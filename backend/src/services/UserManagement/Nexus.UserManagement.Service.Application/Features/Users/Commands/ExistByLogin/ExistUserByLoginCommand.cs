using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Validations.Common.Abstractions;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ExistByLogin
{
    public sealed record class ExistUserByLoginCommand(string Login) : IRequest<Result>, IHasLogin;
}