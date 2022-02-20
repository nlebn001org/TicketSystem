using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        
        //get all users




        //get one user by username

        //create new user with user role  
        //change user info
        //change user password
        
        //get all tickets
        //get all tickets from concrete user

        //change ticket state (solution and state)

        [Route("admin/homepage")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
