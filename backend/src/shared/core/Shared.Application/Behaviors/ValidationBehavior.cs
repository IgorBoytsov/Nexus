using Crossdyne.Toolkit.Results;
using FluentValidation;
using MediatR;
using Shared.Kernel.Errors;

namespace Shared.Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, IResultWithFactory<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(cancellationToken);

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var errors = validationResults
                .SelectMany(results => results.Errors)
                .Where(failure => failure != null)
                .Select(failure => new Error(AppErrors.Validation, failure.ErrorMessage))
                .ToList();

            if (errors.Count != 0)
                return TResponse.CreateFailure(errors);

            return await next(cancellationToken);
        }
    }
}