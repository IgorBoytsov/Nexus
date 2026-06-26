using FluentValidation;

namespace Nexus.UserManagement.Service.Application.Features.Countries.Commands.Create
{
    public class CreateCountyCommandValidator : AbstractValidator<CreateCountryCommand>
    {
        public CreateCountyCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Название страны было пустым.");
        }
    }
}