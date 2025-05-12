using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class StateModel
    {
        [Key]
        public int StateID { get; set; }
        [Required(ErrorMessage ="CountryID is required")]
        public int CountryID { get; set; }
        [Required(ErrorMessage ="StateName is required")]
        [MaxLength(100,ErrorMessage ="StateName cannot exceed 100 characters")]
        public string StateName { get; set; }
        [Required(ErrorMessage ="StateCode is required")]
        public string StateCode { get; set; }
        [Required(ErrorMessage = "CreationDate is required")]
        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage = "UserID is required")]
        public int UserID { get; set; }
    public class CountryDropDownModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }
        public class UserDropDownModel
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
        }

    }
}
