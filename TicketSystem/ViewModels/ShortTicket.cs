using System;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.ViewModels
{
    public class ShortTicket
    {
        public int Id { get; set; }
        public string Creator { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public TicketState TicketState { get; set; }
        public string Solver { get; set; }

    }
}
