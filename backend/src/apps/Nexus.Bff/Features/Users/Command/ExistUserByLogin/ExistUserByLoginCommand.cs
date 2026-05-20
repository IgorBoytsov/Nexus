using Crossdyne.Toolkit.Results;
using MediatR;
using Shared.Validations.Common.Abstractions;

namespace Nexus.Bff.Features.Users.Command.ExistUserByLogin
{
    public sealed record class ExistUserByLoginCommand(string Login) : IRequest<Result>, IHasLogin;
}