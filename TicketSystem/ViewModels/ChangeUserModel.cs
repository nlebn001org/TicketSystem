using System;
using System.Collections.Generic;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.ViewModels
{
    public class ChangeUserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateChanged { get; set; }
        public string RoleName { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
