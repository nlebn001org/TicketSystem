using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.Web.Models;
using TicketSystem.Web.RepositoryServices;

namespace TicketSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly IRepository<User> userRepository;
        readonly SystemDbContext db;

        public HomeController(IRepository<User> userRepository, SystemDbContext db )
        {
            this.userRepository = userRepository;
            this.db = db;

        }

        [Route(""), HttpGet]
        public IActionResult Index()
        {
            ViewData["state"] = User.Identity.Name;
            return View();
        }

        [Route("user/allusers"), HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<User> users = await userRepository.getAllAsync();
            return View(users.ToList());
        }
    }
}
