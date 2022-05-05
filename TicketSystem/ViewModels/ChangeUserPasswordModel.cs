using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Web.ViewModels
{
    public class ChangeUserPasswordModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
