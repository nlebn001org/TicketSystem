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


        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetUserInfo(string username)
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

        public async Task<IActionResult> ChangeUser()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserModel changedUserModel)
        {
            bool result = true;
            User user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == changedUserModel.Username);
            Role role = await _db.Roles.FirstOrDefaultAsync(u => u.RoleName == changedUserModel.RoleName);
            ChangeUserModel ivalidModel = new()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                RoleName = user.Role.RoleName,
                DateChanged = user.DateChanged,
                DateCreated = user.DateCreated
            };
            if (ModelState.IsValid)
            {
                user.Email = changedUserModel.Email != null ? changedUserModel.Email : user.Email;
                user.Name = changedUserModel.Name != null ? changedUserModel.Name : user.Name;
                user.Surname = changedUserModel.Surname != null ? changedUserModel.Surname : user.Surname;
                user.Role = role;
                user.DateChanged = user.Email != changedUserModel.Email ||
                                   user.Name != changedUserModel.Name ||
                                   user.Surname != changedUserModel.Surname ||
                                   user.Role.RoleName != changedUserModel.RoleName
                                   ? DateTime.Now : user.DateChanged;
                try
                {
                    _db.Users.Attach(user);
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    result = false;
                }
                return RedirectToAction("GetChangeResult", "Admin", new { @username = user.Username, @success = result });
            }
            //return RedirectToAction("GetUserInfo", "Admin", new { @username = user.Username });
            return View(ivalidModel);
        }


        public async Task<IActionResult> GetChangeResult(string username, bool success)
        {
            string successResult = success == true ? $"User {username} was successfully changed." : $"User {username} was not changed.";
            return View((object)successResult);
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
