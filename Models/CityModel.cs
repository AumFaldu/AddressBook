using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class CityModel
    {
        [Key]
        public int CityID{get; set;}
        [Required(ErrorMessage ="StateID is required")]
        public int StateID { get; set; }
        [Required(ErrorMessage ="CountryID is required")]
        public int CountryID { get; set; }
        [Required(ErrorMessage ="CityName is required")]
        [MaxLength(100,ErrorMessage ="CityName should not exceed 100 characters")]
        public string CityName { get; set; }
        [Required(ErrorMessage ="STDCode is required")]
        [RegularExpression(@"^\d{3,5}$")]
        public string STDCode { get; set; }
        [Required(ErrorMessage ="PinCode is required")]
        [RegularExpression(@"^\d{6}$")]
        public string PinCode { get; set; }
        [Required(ErrorMessage ="CreationDate is required")]
        public DateTime CreationDate { get; set; }
        [Required(ErrorMessage ="UserID is required")]
        public int UserID { get; set; }
        public class CountryDropDownModel
        {
            public int CountryID { get; set; }
            public string CountryName { get; set; }
        }
        public class StateDropDownModel
        {
            public int StateID { get; set; }
            public string StateName { get; set; }
        }
        public class UserDropDownModel
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
        }
    }
}
