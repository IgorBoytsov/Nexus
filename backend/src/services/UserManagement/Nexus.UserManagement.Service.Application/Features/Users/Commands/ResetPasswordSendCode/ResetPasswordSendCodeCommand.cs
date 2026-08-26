using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Abstractions.Validations;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordSendCode
{
    public sealed record ResetPasswordSendCodeCommand(string Login) : IRequest<Result>, ICommand, IHasLogin;
}