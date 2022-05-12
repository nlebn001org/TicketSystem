using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Web.ViewModels
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password is wrong.")]
        public string ConfirmPassword { get; set; }
    }
}
