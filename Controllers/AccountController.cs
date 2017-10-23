using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WeatherStation.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult External(string provider)
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = "/home/index"
            };

            return new ChallengeResult(provider, authProperties);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");

            return RedirectToAction(nameof(Login));
        }
    }
}