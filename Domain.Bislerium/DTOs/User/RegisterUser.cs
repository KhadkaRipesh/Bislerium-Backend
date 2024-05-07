using Domain.Bislerium.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.DTOs.User
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Username is Required.")]
        [StringLength(50, ErrorMessage = "Username cannot be greator then 50 character long.")]
        [MinLength(6, ErrorMessage = "Username must be less then 6 character.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter password.")]
        [MinLength(8, ErrorMessage = "Password must be greater than 8 character.")]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.BLOGGER;

        public IFormFile? Image { get; set; }
    }
}
