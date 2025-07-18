using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TvBroadCast.Web.Hubs
{
    public class BroadcastHub : Hub
    {

        public async Task NotfiyUpdate()
        {
            await Clients.All.SendAsync("ReceiveUpdate");
        }

    }
}
