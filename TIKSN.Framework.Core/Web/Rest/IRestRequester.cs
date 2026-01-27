namespace TIKSN.Web.Rest;

public interface IRestRequester
{
    public Task<TResult?> RequestAsync<TResult, TRequest>(TRequest request, CancellationToken cancellationToken);
}
