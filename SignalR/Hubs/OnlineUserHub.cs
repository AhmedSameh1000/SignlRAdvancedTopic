using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Models;

namespace SignalR.Hubs
{
    public class OnlineUserHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public OnlineUserHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var connectionId = Context.ConnectionId;
            if(userId !=null)
            {
                var user = new UserConnection()
                {
                    ConnectionId = connectionId,
                    userId = userId
                };

                await _context.AddAsync(user);
               var Completed= await _context.SaveChangesAsync()==0?false:true;

                if (Completed)
                {
                    await Clients.All.SendAsync("userConectedOrDisconected", true);
                }

            }
             await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var connectionId = Context.ConnectionId;
            if (userId != null)
            {
                var user=await _context.Connections.FirstOrDefaultAsync(c=>c.ConnectionId == connectionId);   

                if(user != null)
                {
                    _context.Remove(user);
                    var Completed = await _context.SaveChangesAsync() == 0 ? false : true;
                    if (Completed)
                    {
                        await Clients.All.SendAsync("userConectedOrDisconected", true);
                    }
                }

     
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

}
