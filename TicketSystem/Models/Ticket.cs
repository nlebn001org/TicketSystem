using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.Models
{
    public enum TicketState
    {
        Created = 1,
        Assigned = 2,
        Solved = 3
    }
    public class Ticket : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Describing { get; set; }
        public TicketState TicketState { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? StateChanged { get; set; }
        public string AttachmentPath { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public int? SolverId { get; set; }
        public User Solver { get; set; }
        public string Solution { get; set; }
    }
}
