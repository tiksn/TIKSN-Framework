using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Options;
using TIKSN.Configuration;
using TIKSN.Integration.Messages.Queries;

namespace TIKSN.Integration.Messages;

public class PagingQueryBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IPagingQuery
    where TResponse : notnull
{
    private readonly IOptions<PagingQueryOptions> _options;

    public PagingQueryBehavior(IOptions<PagingQueryOptions> options) => this._options = options ?? throw new ArgumentNullException(nameof(options));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        _ = this._options.Value.MaximumPageSize
            .ToOption()
            .Match(maxPageSize =>
                {
                    if (maxPageSize < 1)
                    {
                        throw new ConfigurationValidationException(
                            "Maximum Page Size for Paging Query is misconfigured, it should be greater than 1");
                    }

                    if (request.Page.Size > maxPageSize)
                    {
                        throw new ValidationException(
                            "Page Size should be less than or equal to Maximum Page Size");
                    }
                },
                () => throw new ConfigurationValidationException("Maximum Page Size for Paging Query is missing"));

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
