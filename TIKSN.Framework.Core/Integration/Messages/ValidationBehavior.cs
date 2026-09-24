using System.Reflection;
using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Integration.Messages;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly Type _validationBehaviorType = typeof(ValidationBehavior<,>);

    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(
        IServiceProvider serviceProvider) => this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        foreach (var requestInterfaceType in request.GetType().GetInterfaces())
        {
            await EnsureValidityAsync(request, requestInterfaceType, this._serviceProvider, cancellationToken)
                .ConfigureAwait(false);
        }

        await EnsureValidityAsync<TRequest, TRequest>(request, this._serviceProvider, cancellationToken)
            .ConfigureAwait(false);

        var response = await next(cancellationToken).ConfigureAwait(false);

        if (response is not null)
        {
            await EnsureValidityAsync<TResponse, TResponse>(response, this._serviceProvider, cancellationToken)
                .ConfigureAwait(false);

            foreach (var responseInterfaceType in response.GetType().GetInterfaces())
            {
                await EnsureValidityAsync(response, responseInterfaceType, this._serviceProvider, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        return response;
    }

    private static async Task EnsureValidityAsync<TR>(
        TR instanceToValidate,
        Type interfaceType,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var validationBehaviorConcreteType =
            _validationBehaviorType.MakeGenericType(typeof(TRequest), typeof(TResponse));
#pragma warning disable S3011
        var ensureValidityMethod = validationBehaviorConcreteType
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
#pragma warning restore S3011
            .Single(x => x.Name == nameof(EnsureValidityAsync) && x.GetGenericArguments().Length == 2);

        var genericEnsureValidityMethod = ensureValidityMethod.MakeGenericMethod(typeof(TR), interfaceType);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await ((Task)genericEnsureValidityMethod.Invoke(null,
        [
            instanceToValidate,
            serviceProvider,
            cancellationToken
        ])).ConfigureAwait(false);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }

    private static async Task EnsureValidityAsync<TR, TV>(
        TR instanceToValidate,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var validators = serviceProvider.GetServices<IValidator<TV>>().ToSeq();

        if (validators.Any())
        {
            var context = new ValidationContext<TR>(instanceToValidate);
            var validationResults = await Task
                .WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }
    }
}
