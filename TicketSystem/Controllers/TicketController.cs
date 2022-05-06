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
            if (ModelState.IsValid)
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
            return View(ticketModel);
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

        [Route("Ticket/GetTicket/{id:int}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            Ticket ticket = await _db.Tickets.Include(u => u.Creator).Include(u => u.Solver).FirstOrDefaultAsync(u => u.Id == id);
            TicketInfoModel ticketModel = new()
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Describing,
                TicketState = ticket.TicketState,
                DateCreated = ticket.DateCreated,
                StateChanged = ticket.StateChanged,
                CreatorName = ticket.Creator.Username,
                SolverName = ticket.Solver?.Username,
                Solution = ticket.Solution
            };
            return View(ticketModel);
        }
        [HttpPost]
        public async Task<IActionResult> GetTicket()
        {
            return View();
        }

        [Route("Ticket/ClosedByCustomer/{id:int}")]
        public async Task<IActionResult> CloseTicketByCustomer(int id)
        {
            try
            {
                Ticket ticket = await _db.Tickets.FirstOrDefaultAsync(u => u.Id == id);
                ticket.StateChanged = DateTime.Now;
                ticket.TicketState = TicketState.Solved;
                ticket.Solution = $"Ticket was close by requestor at {DateTime.Now}";
                _db.Update(ticket);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            string result = $"Ticket with ID {id} is closed by yourself.";
            return View((object)result);
        }



        [HttpPost]
        public async Task<IActionResult> CloseTicketWithSolution(CloseTicketModel model)
        {
            string solver = User.Identity.Name;
            string yourself = "yourself";
            Ticket ticket = new();
            string result = string.Empty;
            try
            {
                ticket = await _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .FirstOrDefaultAsync(u => u.Id == model.Id);

                if (User.IsInRole("admin"))
                {
                    ticket.Solver = await _db.Users.FirstOrDefaultAsync(u => u.Username == solver);
                    if (ticket.Solver.Id == ticket.Creator.Id)
                    {
                        ticket.Solver = null;
                        solver = "yourself";
                    }
                }

                ticket.StateChanged = DateTime.Now;
                ticket.TicketState = TicketState.Solved;
                ticket.Solution = $"{model.Solution}";
                _db.Update(ticket);
                await _db.SaveChangesAsync();
                result = $"Ticket with ID {ticket.Id} is closed by {solver}.";
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return View((object)result);
        }
    }
}
