using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WiredBrain.Helpers;

namespace WiredBrain.Hubs
{
    //[Authorize]
    public class CoffeeHub: Hub
    {
        private readonly OrderChecker _orderChecker;

        public CoffeeHub(OrderChecker orderChecker)
        {
            _orderChecker = orderChecker;
        }

        public async Task GetUpdateForOrder(int orderId)
        {
            //Context.User.
            CheckResult result;
            do
            {
                result = _orderChecker.GetUpdate(orderId);
                Thread.Sleep(1000);
                if (result.New)
                    await Clients.Caller.SendAsync("ReceiveOrderUpdate", 
                        result.Update);
            } while (!result.Finished);
            await Clients.Caller.SendAsync("Finished");
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;  // every where in the hub, your have access to Context property
            //await Clients.Client(connectionId).SendAsync("NewOrder", order);
            //await Clients.AllExcept(connectionId).SendAsync();
            //await Groups.AddToGroupAsync(connectionId, "AmericanoGroup");
            await Groups.RemoveFromGroupAsync(connectionId, "AmericanoGroup");
        }
    }
}
