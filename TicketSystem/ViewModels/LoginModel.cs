using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is not specified.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is not specified.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
