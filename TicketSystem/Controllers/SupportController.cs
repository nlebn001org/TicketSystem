using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
