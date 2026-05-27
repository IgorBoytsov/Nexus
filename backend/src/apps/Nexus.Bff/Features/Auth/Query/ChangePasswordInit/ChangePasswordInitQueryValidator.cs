using FluentValidation;
using Shared.Validations.Common.Validators;

namespace Nexus.Bff.Features.Auth.Query.ChangePasswordInit
{
    public sealed class ChangePasswordInitQueryValidator : AbstractValidator<ChangePasswordInitQuery>
    {
        public ChangePasswordInitQueryValidator()
        {
            Include(new GuidUserIdValidator());
        }
    }
}