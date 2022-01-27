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
        public async IAsyncEnumerable<int> Counter(
        int count,
        int delay,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
        {
            for (var i = 0; i < count; i++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();

                yield return i;

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(delay, cancellationToken);
            }
        }
        public ChannelReader<int> DelayCounter(int delay)
        {
            var channel = Channel.CreateUnbounded<int>();

            _ = WriteItems(channel.Writer, 20, delay);

            return channel.Reader;
        }

        private async Task WriteItems(ChannelWriter<int> writer, int count, int delay)
        {
            for (var i = 0; i < count; i++)
            {
                await writer.WriteAsync(i);
                await Task.Delay(delay);
            }

            writer.TryComplete();
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
