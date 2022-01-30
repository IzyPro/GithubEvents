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
            //calls the get service with the fetch events route and query parameters 
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
                    //Stream each event to the client individually
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
