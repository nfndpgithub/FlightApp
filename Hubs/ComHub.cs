using Microsoft.AspNetCore.SignalR;
using vezba.Model;
namespace vezba.Hubs
{
    public class ComHub : Hub
    {
        public async Task SendRequest(Flight flight)
        {
            await Clients.All.SendAsync("SendRequest", flight);
        }
        public async Task Answer(string id, bool answer)
        {
            await Clients.All.SendAsync("Answer", id, answer);
        }





    }
}
