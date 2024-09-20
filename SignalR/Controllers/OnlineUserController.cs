using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Data;
using SignalR.Hubs;

namespace SignalR.Controllers
{
    public class OnlineUserController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IHubContext<OnlineUserHub> hubContext;

        public OnlineUserController(ApplicationDbContext applicationDbContext, IHubContext<OnlineUserHub> hubContext)
        {
            this.applicationDbContext = applicationDbContext;
            this.hubContext = hubContext;
        }
        public  IActionResult Index()
        {       
            return View();
        }

        public async Task<IActionResult> GetOnlineUsers()
        {
            var users = await applicationDbContext.Users
              .Select(u => new OnlineUsers
              {
                  userId = u.Id,
                  userName = u.UserName,
                  Email = u.Email,
                  isOnline = applicationDbContext.Connections.Any(uc => uc.userId == u.Id),

              })
              .ToListAsync();

            return Ok(users);
        }


       
        public async Task<IActionResult> logoutUser([FromQuery]string userId)
        {
            
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(c => c.Id == userId);


            if (user!=null)
            {
                user.ChnagedGuid = Guid.NewGuid();
                applicationDbContext.Users.Update(user);
                await applicationDbContext.SaveChangesAsync();
            }
            var AllConnectionStringsForThisUser = await applicationDbContext.Connections
                .Where(c => c.userId == userId).Select(c => c.ConnectionId).ToListAsync();
            await hubContext.Clients.Clients(AllConnectionStringsForThisUser).SendAsync("LogOutThisUser", true);
            return Ok();
        }
    }
    public class OnlineUsers
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string Email { get; set; }
        public bool isOnline { get; set; }
    }
}
