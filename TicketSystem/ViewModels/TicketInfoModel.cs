using System;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.ViewModels
{
    public class TicketInfoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketState TicketState { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? StateChanged { get; set; }
        //public string AttachmentPath { get; set; }
        public string CreatorName { get; set; }
        public string SolverName { get; set; }
        public string Solution { get; set; }
    }
}
