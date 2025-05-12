using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        [MaxLength(100,ErrorMessage ="UserName cannot exceed 100 characters")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "MobileNo is required")]
        [RegularExpression(@"^\d{10}$")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage ="EmailID is required")]
        [EmailAddress(ErrorMessage ="Incorrect format of Email Address")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "CreationDate is required")]
        public DateTime CreationDate { get; set; }
    }
}
