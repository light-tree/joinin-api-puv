using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.User
{
    public class UserRequestDTO
    {
        [Required]
        Guid id;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Full name must be between {2} and {1} characters long.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Birthday is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDay { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Description must be at least {2} characters long.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Skill is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Skill must be between {2} and {1} characters long.")]
        public string Skill { get; set; }

        [StringLength(100, ErrorMessage = "Other contact must be less than {1} characters long.")]
        public string OtherContact { get; set; }

        [StringLength(200, ErrorMessage = "Avatar must be less than {1} characters long.")]
        public string Avatar { get; set; }

        [StringLength(200, ErrorMessage = "Theme must be less than {1} characters long.")]
        public string Theme { get; set; }

        [Required(ErrorMessage = "User must have at least one major.")]
        public List<Guid> MajorIdList { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public PlatForm ?PlatForm { get; set; } 



    }
}
