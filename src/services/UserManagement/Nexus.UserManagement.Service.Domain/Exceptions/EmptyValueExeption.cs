using Quantropic.Toolkit.Results;
using Shared.Kernel.Exceptions;

namespace Nexus.UserManagement.Service.Domain.Exceptions
{
    public sealed class EmptyValueExeption(Error error) : DomainException(error)
    {
    }
}