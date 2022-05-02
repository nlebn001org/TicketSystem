using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicketSystem.Web.Models;
using TicketSystem.Web.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;



namespace TicketSystem.Web.Controllers
{
    public class TicketController : Controller
    {

        readonly SystemDbContext _db;

        public TicketController(SystemDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewTicket()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewTicket(NewTicketModel ticketModel)
        {
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            Ticket ticket = new()
            {
                Title = ticketModel.Title,
                Describing = ticketModel.Description,
                Creator = user,
                DateCreated = DateTime.Now
            };
            await _db.Tickets.AddAsync(ticket);
            await _db.SaveChangesAsync();
            return RedirectToAction("GetCreationResult", "Ticket", new { @id = ticket.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetCreationResult(int? id)
        {
            string result = $"Ticket was created with ID {id}";
            return View((object)result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTickets()
        {
            List<ShortTicket> tickets = new();
            User user = await _db.Users.Include(u => u.Tickets).FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            foreach (Ticket ticket in user.Tickets)
            {
                tickets.Add(new ShortTicket
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    DateCreated = ticket.DateCreated,
                    TicketState = ticket.TicketState
                });
            }
            return View(tickets);
        }

    }
}
