using GithubEvents.Helpers;
using GithubEvents.Interfaces;
using GithubEvents.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace GithubEvents.Hubs
{
    public class EventHub : Hub
    {
        private IHttpService _httpService;
        public EventHub(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async IAsyncEnumerable<EventModel> EventsStream(int delay, [EnumeratorCancellation] CancellationToken cancellationToken, int? pageNumber = 1)
        {
            var response = await _httpService.getHttp(RouteHelper.getEvents, $"page={pageNumber}&per_page={int.MaxValue}");
            if(response != null)
            {
                if(response?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var events = new List<EventModel>();
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        events = JsonConvert.DeserializeObject<List<EventModel>>(response.Content);
                    }
                    catch (Exception ex)
                    {

                    }
                    foreach(var e in events)
                    {
                        yield return e;
                        await Task.Delay(delay, cancellationToken);
                    }
                }
            }
        }
    }
}
