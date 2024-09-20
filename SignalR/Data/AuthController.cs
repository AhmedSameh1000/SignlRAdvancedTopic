using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SignalR.Migrations;
using System.Security.Claims;

namespace SignalR.Data
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IEmailSender _emailSender;

        public AuthController(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult LogIn()
        {
            return View();
        } 
        public async Task <IActionResult> LogIn(logInModel logInModel)
        {
            var user = await _userManager.FindByNameAsync(logInModel.email);
            if (user is null)
                return View();
            var isSuccessPassword = await _userManager.CheckPasswordAsync(user, logInModel.Password);



            user.ChnagedGuid = Guid.NewGuid();
            await _userManager.UpdateAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("ChnagedGuid",user.ChnagedGuid.ToString())
            };

            await _signInManager.SignInWithClaimsAsync(user, logInModel.RememberMe, claims);


            if (!isSuccessPassword)
                return View();

            if (isSuccessPassword)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }  
        public async Task<IActionResult> Register(RegisterInModel registerInModel)
        {
            var user = new AppUser();

            await _userStore.SetUserNameAsync(user, registerInModel.email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, registerInModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(LogIn));
            }

            return View(registerInModel);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

    }
    public class logInModel
    {
        public string email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    } 
    public class RegisterInModel
    {
        public string email { get; set; }
        public string Password { get; set; }
    }

}


