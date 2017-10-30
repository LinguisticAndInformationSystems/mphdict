using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphweb
{
    public class TextAnalyzeHub:Hub
    {
        public async Task Send(string message)
        {
            //first argument - recipient method name 
            //await this.Clients.All.InvokeAsync("Send", message);
            await this.Clients.Client(Context.ConnectionId).InvokeAsync("Send", message);
        }
    }
}
