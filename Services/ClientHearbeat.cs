using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;
using WindTalkerMessenger.Hubs;
using System.Timers;
using WindTalkerMessenger.Models;

namespace WindTalkerMessenger.Services
{
    public class ClientHearbeat : IInvocable
    {

        //https://jonhilton.net/coravel/
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ClientHearbeat> _logger;
        private readonly OnlineUsersLists _onlineUsers;
        private readonly HeartBeat _heartBeat;
        public ClientHearbeat(IHubContext<ChatHub> hubContext, 
                              ILogger<ClientHearbeat> logger,
                              OnlineUsersLists onlineUsers,
                              HeartBeat heartBeat)
        {
            _hubContext = hubContext;
            _logger = logger;
            _onlineUsers = onlineUsers;
            _heartBeat = heartBeat;
        }
        public async Task Invoke()
        {
            var tree = _onlineUsers.clientHeartBeatTree.ToList();

            //https://docs.coravel.net/Scheduler/
            await _hubContext.Clients.All.SendAsync("heartbeat");
            foreach (ClientNode node in tree)
            {
                _heartBeat.HeartBeatAdd(node.NodeName.ToString());

            }
            _logger.LogInformation("Pulses Sent");

            //return Task.CompletedTask;

        }

    }
}

