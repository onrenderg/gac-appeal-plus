using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class UserList
    {
        public List<UserListD> UserListDetails { get; set; }
        public List<EmpListD> EmpListDetails { get; set; }
    }
    public class UserListD
    {
        [Required(ErrorMessage = "Enter User ID!")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Only 10 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Enter User Name!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter Designation!")]
        public string Designation { get; set; }
        [Required(ErrorMessage = "Enter Mobile No!")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Please enter a valid mobile number!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter a valid mobile number!")]
        public string Mobile { get; set; }
        public string isActive { get; set; }
        public string RoleName { get; set; }
        [Required(ErrorMessage = "Enter user Address!")]
        public string UserAddress { get; set; }

        public string RoleTypeID { get; set; }

        [Required(ErrorMessage = "Select Role ")]
        public string UserRoleID { get; set; }

        //[Required(ErrorMessage = "Select GAC Type")]
        public string GACID { get; set; }
        public string GACIDString { get; set; }


        public List<SelectListItem> RoleTypeList { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public List<SelectListItem> GACTypeList { get; set; }
    }


    public class VerifyOtp
    {
        [Required(ErrorMessage = "Enter User ID!")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Only 10 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Enter User Name!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter Mobile No!")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Please enter a valid mobile number!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter a valid mobile number!")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Enter OTP")]

        [StringLength(6, MinimumLength = 6, ErrorMessage = "Please enter a valid OTP")]
        public string mobOTP { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Please enter a valid Password")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Enter Confirm Password")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Please enter a valid Confirm Password")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "Password and Confirmation Password Must Match!")]
        public string ConfPassword { get; set; }

        public string HasPass { get; set; }
        public string Seed { get; set; }




    }

    public class ChangePassword
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        //[MinLength(8, ErrorMessage = "Min. 8 characters allowed!")]
        [StringLength(15, ErrorMessage = "Max. 15 characters allowed!")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [MinLength(8, ErrorMessage = "Min. 8 characters allowed!")]
        [StringLength(15, ErrorMessage = "Max. 15 characters allowed!")]

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*(_|[^\w])).+$", ErrorMessage = "The password does not comply with the password policy. Please check the Password requirements!")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [MinLength(8, ErrorMessage = "Min. 8 characters allowed!")]
        [StringLength(15, ErrorMessage = "Max. 15 characters allowed!")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string HashCurrentPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string HashNewPassword { get; set; }
    }
    public class EmpListD
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string StoreNotification { get; set; }
        public string AutoClearInterval { get; set; }
        public string AppRegistered { get; set; }
    }
}