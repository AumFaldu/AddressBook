using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class CountryModel
    {
        [Key]
        public int CountryID { get; set; }
        [Required(ErrorMessage ="CountryName is required")]
        [MaxLength(100,ErrorMessage ="CountryName cannot exceed 100 characters")]
        public string CountryName { get; set; }
        [Required(ErrorMessage ="CountryCode is required")]
        [Range(1, 999,ErrorMessage ="CountryCode should be in range 1-999")]
        public string CountryCode { get; set; }
        [Required(ErrorMessage ="CreationDate is required")]
        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage ="UserID is required")]
        public int UserID { get; set; }
        public class UserDropDownModel
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
        }
    }
}
