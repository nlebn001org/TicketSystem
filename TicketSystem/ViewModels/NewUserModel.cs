using System.ComponentModel.DataAnnotations;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.ViewModels
{
    public class NewUserModel
    {
        [Required(ErrorMessage = "Username is not specified.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is not specified.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [EmailAddress(ErrorMessage = "This email is not valid.")]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string RoleName { get; set; }

    }
}
