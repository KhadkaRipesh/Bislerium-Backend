using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.DTOs.Notification
{
    public record CreateToken
    {
        public string? Token { get; set; } = string.Empty;
        public Guid UserID { get; set; }
    }
}
