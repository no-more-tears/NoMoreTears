using FluentValidation;
using MediatR;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for validating incoming requests 
///     using registered FluentValidation validators.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators): IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    /// <summary> Collection of validators for the current request type. </summary>У
    private readonly IReadOnlyList<IValidator<TRequest>> _validators =
        validators as IReadOnlyList<IValidator<TRequest>> ?? validators.ToList();

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // If there are no validators, continue the pipeline execution.
        if (_validators.Count == 0)
        {
            return await next(cancellationToken);
        }

        var validationFailures = await CollectFailuresAsync(_validators, request);

        // If any failures exist, throw exception with aggregated error message.
        if (validationFailures.Length > 0)
        {
            var errorMessage = BuildErrorMessage(validationFailures);
            throw new ValidationException(errorMessage, validationFailures);
        }

        return await next(cancellationToken);
    }

    /// <summary>
    ///     Executes all validators and collects validation failures.
    /// </summary>
    private static async Task<ValidationFailure[]> CollectFailuresAsync(
        IReadOnlyList<IValidator<TRequest>> validators,
        TRequest request)
    {
        var context = new ValidationContext<TRequest>(request);
        List<ValidationFailure>? validationFailures = null;

        foreach (var validator in validators)
        {
            var validationResult = await validator.ValidateAsync(context);

            if (!validationResult.IsValid)
            {
                validationFailures ??= new(validationResult.Errors.Count);
                validationFailures.AddRange(validationResult.Errors);
            }
        }

        return validationFailures?.ToArray() ?? [];
    }

    /// <summary>
    ///     Builds a formatted error message string from a collection of validation failures.
    /// </summary>
    private static string BuildErrorMessage(IEnumerable<ValidationFailure> failures)
    {
        return string.Join(
            Environment.NewLine,
            failures.Select(f => $"Field '{f.PropertyName}': {f.ErrorMessage}"));
    }
}