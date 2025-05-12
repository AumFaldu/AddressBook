using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage ="UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "EmailID is required")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "MobileNo is required")]
        [Phone]
        public string MobileNo { get; set; }
    }
}
