using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.Web.Models;
using TicketSystem.Web.ViewModels;

namespace TicketSystem.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        readonly SystemDbContext _db;
        public AdminController(SystemDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult UserManaging() //form
        {
            return View(new List<FindUserModel>());
        }

        [HttpPost]
        public async Task<IActionResult> UserManaging(List<FindUserModel> models)
        {
            List<User> users = new();
            List<FindUserModel> userModels = new();
            FindUserModel model = models.FirstOrDefault();

            if (!(model.RoleName == null && model.Username == null))
            {
                if (ModelState.IsValid)
                {
                    if (model.Username == null && model.RoleName != null)
                    {
                        users = await _db.Users.Include(u => u.Role).
                            Where(u => u.Role.RoleName == model.RoleName).ToListAsync();
                    }
                    if (model.RoleName == null && model.Username != null)
                    {
                        users = await _db.Users.Include(u => u.Role).
                            Where(u => u.Username == model.Username).ToListAsync();
                    }
                    if (model.Username != null && model.RoleName != null)
                    {
                        users = await _db.Users.Include(u => u.Role).
                            Where(u => u.Username == model.Username && u.Role.RoleName == model.RoleName)
                            .ToListAsync();
                    }
                }

                foreach (User user in users)
                    userModels.Add(new FindUserModel
                    { Username = user.Username, RoleName = user.Role.RoleName });
            }
            return View(userModels);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(string username)
        {
            User user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
            ChangeUserModel changeUserModel = new()
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                RoleName = user.Role.RoleName,
                DateCreated = user.DateCreated,
                DateChanged = user.DateChanged,
                Tickets = user.Tickets
            };

            return View(changeUserModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTickets()
        {
            List<ShortTicket> shortTickets = new();
            return View(shortTickets);
        }


        [HttpPost]
        public async Task<IActionResult> GetUserTickets(string username)
        {
            User user = await _db.Users.Include(u => u.Tickets).Include(u => u.Ticket).
                FirstOrDefaultAsync(u => u.Username == username);

            List<Ticket> tickets = user.Tickets.ToList();
            List<ShortTicket> shortTickets = new();
            foreach (Ticket ticket in tickets)
            {
                shortTickets.Add(new ShortTicket
                {
                    Id = ticket.Id,
                    Creator = ticket.Creator.Username,
                    Title = ticket.Title,
                    DateCreated = ticket.DateCreated,
                    TicketState = ticket.TicketState,
                    Solver = ticket.Solver?.Username        //TODO solve the issue (user.Tickets[].Solver is always null )
                });
            }
            return View(shortTickets);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser(NewUserModel userModel)
        {
            bool isExist = true;
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    Username = userModel.Username,
                    Password = userModel.Password,
                    Email = userModel.Email,
                    Name = userModel.Name,
                    Surname = userModel.Surname,
                    Role = await _db.Roles.FirstOrDefaultAsync(u => u.RoleName == userModel.RoleName),
                    DateCreated = DateTime.Now
                };

                isExist = await _db.Users.AnyAsync(u => u.Username == userModel.Username);

                if (isExist)
                {
                    ModelState.AddModelError("", "User with this username already exists.");
                    return View(userModel);
                }

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("", "Model is incorrect.");
                return View(userModel);
            }
            return RedirectToAction("GetCreationResult", "Admin", new { @username = userModel.Username });
        }

        [HttpGet]
        public async Task<IActionResult> GetCreationResult(string username)
        {
            string message = $"User with {username} was created.";
            return View((object)message);
        }


        //TODO sign out the user if he is signed in. 
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            int _id = int.Parse(id);
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _id);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("UserManaging", "Admin");
        }

    }
}
