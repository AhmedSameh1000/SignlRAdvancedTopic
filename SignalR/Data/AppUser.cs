using Microsoft.AspNetCore.Identity;

namespace SignalR.Data
{
    public class AppUser:IdentityUser
    {

        public Guid ChnagedGuid {  get; set; }
    }
}
