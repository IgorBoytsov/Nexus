using Quantropic.Toolkit.Results;
using Shared.Kernel.Exceptions;

namespace Nexus.UserManagement.Service.Domain.Exceptions
{
    public sealed class UserAuthenticatorException(Error error) : DomainException(error)
    {
    }
}