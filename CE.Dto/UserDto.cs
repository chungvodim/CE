using CE.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CE.Dto
{
    public class UserDto : BaseDto
    {
        [Key]
        public int UserID { get; set; }
        public override int ID { get { return UserID; } }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Required]
        [Display(Name = "Language")]
        public int LocaleID { get; set; }
        [Required]
        [Display(Name = "Company")]
        public int CompanyID { get; set; }
        [Required]
        [Display(Name = "Branch")]
        public int? BranchID { get; set; }
        public string BranhchC2CUID { get; set; }
        [Required]
        [Display(Name = "Role")]
        public List<int> RoleIds { get; set; }
        [Display(Name = "Role")]
        public int PrimaryRoleId
        {
            get
            {
                int roleId = (int)UserRole.Client;
                if (RoleIds != null && RoleIds.Any())
                {
                    roleId = RoleIds.First();
                }
                return roleId;
            }
            set
            {
                RoleIds.Insert(0, value);
            }
        }

        public List<string> Roles { get; set; }
        public string PrimaryRole { get; set; }
        public string Gender { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string BranchPhoneNumber { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public Enum.UserStatus UserStatus { get; set; }
        public int? CreatedUserID { get; set; }
        public int? UpdatedUserId { get; set; }
        public string BeautifulMessage { get; set; }
        public string SecurityStamp { get; set; }

        public UserDto()
        {
            RoleIds = new List<int>();
            Roles = new List<string>();
        }
    }
}
