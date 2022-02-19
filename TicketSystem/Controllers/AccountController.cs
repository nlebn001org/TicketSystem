using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketSystem.Web.Models;
using TicketSystem.Web.ViewModels;

namespace TicketSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        readonly SystemDbContext db;

        public AccountController(SystemDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => model.Username == u.Username && model.Password == u.Password);
                if (user != null)
                {
                    await Authenticate(user); //authentication
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect Username or Password");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    User newUser = new User { Username = model.Username, Password = model.Password }; //could be registered only customer
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == "customer");
                    if (userRole != null) newUser.Role = userRole;
                    await db.Users.AddAsync(newUser);
                    await db.SaveChangesAsync();
                    await Authenticate(newUser); //authentication
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Probably user with this username is already exists.");
                }
            }
            return View(model);
        }


        // TODO: ADD CHANGE PASSWORD CONTROLLER


        private async Task Authenticate(User user)
        {
            //create one claim
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.RoleName)
            };

            //create ClaimsIdentity object
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            //set up the cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}
