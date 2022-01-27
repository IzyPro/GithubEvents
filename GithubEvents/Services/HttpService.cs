using GithubEvents.Interfaces;
using GithubEvents.Models;
using RestSharp;

namespace GithubEvents.Services
{
    public class HttpService : IHttpService
    {
        private readonly IConfiguration _configuration;
        private string _baseURL;
        private string _accept;

        public HttpService(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseURL = _configuration["AppSettings:BaseURL"];
            _accept = _configuration["AppSettings:Accept"];
        }
        public async Task<RestResponse?> getHttp(string url, string queries)
        {
            try
            {
                Dictionary<string, string> queryParams = GetQueries(queries);
                var client = new RestClient(_baseURL);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("accept", _accept);
                foreach(var query in queryParams)
                {
                    request.AddParameter(query.Key, query.Value);
                }
                return await client.ExecuteAsync(request);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }
        private Dictionary<string, string> GetQueries(string queryString)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            var queryList = queryString.Split('&').ToList();
            foreach (var query in queryList)
            {
                var keyAndValue = query.Split('=');
                keyValuePairs.Add(keyAndValue[0], keyAndValue[1]);
            }
            return keyValuePairs;
        }
    }
}
