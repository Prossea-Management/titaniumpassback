using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace titaniumpassback.Models
{
    public class User : IdentityUser<int>
    {
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public virtual Company Company { get; set; }
    }
    public class UserVM
    {
        public int? User { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CompanyName { get; set; }
    }
    public class UserLogInVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserTokenVM
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Role { get; set; }
    }
    public class ResponseUserVM
    {
        public User user { get; set; }
        public string Message { get; set; }
    }
    public class UserLoguedVM
    {
        public User userLogued { get; set; }
    }
    public class ResetPasswordVM
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }

    public class ForgotPasswordVM
    {
        public string Email { get; set; }
    }
}
