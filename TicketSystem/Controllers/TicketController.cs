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
        [Authorize(Roles = "admin, support")]
        public async Task<IActionResult> GetTicketList(List<ShortTicket> model)
        {
            if (model == null || model.Count == 0) return View(new List<ShortTicket>());

            List<ShortTicket> resultList = new();

            int _id = model[0].Id;
            string _creator = model[0]?.Creator;
            string _solver = model[0]?.Solver;
            TicketState _state = model[0].TicketState;

            //all default
            if (_id == default && _creator == default && _solver == default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator).ToList();

                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }

            // id
            if (_id != default && _creator == default && _solver == default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator).Where(u => u.Id == _id);
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // id && creator
            if (_id != default && _creator != default && _solver == default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Id == _id) && (u.Creator.Username == _creator));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // id && solver
            if (_id != default && _creator == default && _solver != default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Id == _id) && (u.Solver.Username == _solver));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // id && creator && solver
            if (_id != default && _creator != default && _solver != default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Id == _id) && (u.Creator.Username == _creator) && (u.Solver.Username == _solver));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // id && creator && solver && state
            if (_id != default && _creator != default && _solver != default && _state != default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Id == _id) && (u.Creator.Username == _creator) && (u.Solver.Username == _solver) && (u.TicketState == _state));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // id && state
            if (_id != default && _creator == default && _solver == default && _state != default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Id == _id) && (u.TicketState == _state));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }

            // creator
            if (_id == default && _creator != default && _solver == default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => u.Creator.Username == _creator);
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // creator && solver
            if (_id == default && _creator != default && _solver != default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Creator.Username == _creator) && (u.Solver.Username == _solver));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // creator && solver && state
            if (_id == default && _creator != default && _solver != default && _state != default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Creator.Username == _creator) && (u.Solver.Username == _solver) && (u.TicketState == _state));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // creator && state
            if (_id == default && _creator != default && _solver == default && _state != default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Creator.Username == _creator) && (u.TicketState == _state));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }

            // solver
            if (_id == default && _creator == default && _solver != default && _state == default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Solver.Username == _solver));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }
            // solver && state
            if (_id == default && _creator == default && _solver != default && _state != default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.Solver.Username == _solver) && (u.TicketState == _state));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }

            // state
            if (_id == default && _creator == default && _solver == default && _state != default)
            {
                IEnumerable<Ticket> tickets = _db.Tickets.Include(u => u.Solver).Include(u => u.Creator)
                    .Where(u => (u.TicketState == _state));
                foreach (Ticket ticket in tickets)
                {
                    resultList.Add(new ShortTicket()
                    {
                        Id = ticket.Id,
                        Creator = ticket.Creator?.Username,
                        Solver = ticket.Solver?.Username,
                        TicketState = ticket.TicketState
                    });
                }
                return View(resultList);
            }

            return View();
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

        [Route("Ticket/AssignOnMe/{id:int}")]
        [Authorize(Roles = "admin, support")]
        public async Task<IActionResult> AssignOnMe(int id)
        {
            string result = null;
            try
            {
                Ticket ticket = await _db.Tickets.Include(u => u.Solver).FirstOrDefaultAsync(u => u.Id == id);
                User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);

                ticket.Solver = user;
                ticket.TicketState = TicketState.Assigned;

                _db.Update(ticket);
                await _db.SaveChangesAsync();

                result = $"You were assigned to ticket with ID {id}";
            }
            catch (Exception)
            {
                result = $"You were not assigned to ticket with ID {id}. Something went wrong.";
            }
            return View((object)result);
        }

        [Route("Ticket/UnassignFromMe/{id:int}")]
        [Authorize(Roles = "admin, support")]
        public async Task<IActionResult> UnassignFromMe(int id)
        {
            string result = null;
            try
            {
                Ticket ticket = await _db.Tickets.Include(u => u.Solver).FirstOrDefaultAsync(u => u.Id == id);

                ticket.Solver = null;
                ticket.TicketState = TicketState.Created;

                _db.Update(ticket);
                await _db.SaveChangesAsync();

                result = $"You were unassigned from ticket with ID {id}";
            }
            catch (Exception)
            {
                result = $"You were not unassigned to ticket with ID {id}. Something went wrong.";
            }
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
