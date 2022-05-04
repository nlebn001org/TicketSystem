using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.ViewModels
{
    public class ChangeUserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is not specified.")]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage = "This email is not valid.")]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateChanged { get; set; }
        public string RoleName { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
