namespace TIKSN.Web.Rest;

public interface IRestAuthenticationTokenProvider
{
    Task<string> GetAuthenticationTokenAsync(string apiKey);
}
