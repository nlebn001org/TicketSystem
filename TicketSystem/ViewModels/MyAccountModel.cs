using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.ViewModels
{
    public class MyAccountModel
    {
        public string Username { get; set; }
        public Role Role { get; set; }
        [EmailAddress(ErrorMessage = "Your mail is not valid.")]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        //todo: add option to change a photo
        //public string PhotoPath { get; set; }

        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password is wrong.")]
        public string ConfirmPassword { get; set; }
    }
}
