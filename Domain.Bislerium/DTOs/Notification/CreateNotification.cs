using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.DTOs.Notification
{
    public record CreateNotification
    {
        [Required]
        [MinLength(1, ErrorMessage = "Comment should be greater than 1 characters")]
        [MaxLength(225, ErrorMessage = "Comment Must Be Less Then 225 Character Long.")]
        public string Body { get; set; } = string.Empty;

        [Required]
        public Guid UserID { get; set; }
    }
}
