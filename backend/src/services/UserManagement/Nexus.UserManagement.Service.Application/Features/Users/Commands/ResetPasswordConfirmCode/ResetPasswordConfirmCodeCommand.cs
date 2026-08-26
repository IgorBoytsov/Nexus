using MediatR;
using Crossdyne.Toolkit.Results;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Shared.Abstractions.Validations;
using Nexus.UserManagement.Service.Application.Abstractions.Validators;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode
{
    public sealed record ResetPasswordConfirmCodeCommand(string Login, string Code) : IRequest<Result>, ICommand, IHasLogin, IHasCode;
}