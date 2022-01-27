using RestSharp;

namespace GithubEvents.Interfaces
{
    public interface IHttpService
    {
        public Task<RestResponse?> getHttp(string url, string queries);
    }
}
