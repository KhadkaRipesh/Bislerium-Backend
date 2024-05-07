using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.DTOs.User
{
    public class UpdateProfile
    {
        public string? UserName { get; set; } = string.Empty;
        public IFormFile? Image { get; set; } = null;
    }
}
