using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using vCore_MVC.Models;

namespace vCore_MVC.Controllers
{
    public class HomeController : Controller
    {
        // DI for AppDbContext
        private readonly AppDbContext _context; // _Context is 
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index"); // already logged in
            }

            return View();
        }

        [HttpPost]  
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = _context.CoreUsers
                .FirstOrDefault(u => u.UserName == model.UserName
                                  && u.Password == model.Password);

            if (user != null)
            {
                var fullName = $"{user.FirstName} {user.LastName}".Trim();
                if (string.IsNullOrWhiteSpace(fullName))
                    fullName = user.UserName;

                var role = _context.CoreRoles
         .Where(r => r.Id == user.RoleId)
         .Select(r => r.RoleName)
         .FirstOrDefault();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("FullName", fullName),
                    new Claim(ClaimTypes.Role, role ?? "User"),
                    new Claim("TenantId", user.TenantId.ToString())
                };
               
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);


                await HttpContext.SignInAsync
                    ("MyCookieAuth", principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,


                    ExpiresUtc = model.RememberMe
                                ? DateTime.UtcNow.AddDays(7)    // with remember me , expire in 7 days
                                : DateTime.UtcNow.AddMinutes(30),  // without remember me, expire in 30 minutes

                    AllowRefresh = true
                }
                    );

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Home");
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
       

    }
}
