using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.DTOs.User
{
    public class ChangePassword
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(4, ErrorMessage = "New Password Must Be Greater than 8 Character.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(4, ErrorMessage = "Confirm Password Must be Greater Then 8 Character.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
