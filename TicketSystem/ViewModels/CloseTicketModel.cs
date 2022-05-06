using TicketSystem.Web.Models;

namespace TicketSystem.Web.ViewModels
{
    public class CloseTicketModel
    {
        public int Id { get; set; }
        public string Solution { get; set; }
        public User Solver { get; set; }
    }
}
