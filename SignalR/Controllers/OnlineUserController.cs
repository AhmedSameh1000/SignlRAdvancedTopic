using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalR.Data;

namespace SignalR.Controllers
{
    public class OnlineUserController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public OnlineUserController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
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


        public async Task<IActionResult> logoutUser(string userId)
        {
            await Task.CompletedTask;
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
