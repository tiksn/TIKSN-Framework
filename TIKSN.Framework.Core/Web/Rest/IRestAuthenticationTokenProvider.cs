namespace TIKSN.Web.Rest;

public interface IRestAuthenticationTokenProvider
{
    public Task<string> GetAuthenticationTokenAsync(string apiKey);
}
