using Microsoft.AspNetCore.Http;

namespace TicketSystem.Web.ViewModels
{
    public class NewTicketModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        //public IFormFile Attachment { get; set; }
    }
}
