namespace TIKSN.Web.Rest;

public interface IRestRequester
{
    Task<TResult?> RequestAsync<TResult, TRequest>(TRequest request, CancellationToken cancellationToken);
}
