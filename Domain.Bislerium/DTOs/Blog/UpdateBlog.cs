using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.DTOs.Blog
{
    public record UpdateBlog
    {
        [MinLength(5, ErrorMessage = "Title should be greater than 5 characters")]
        public string? Title { get; set; } = string.Empty;


        [MinLength(5, ErrorMessage = "Summary should be greater than 5 characters")]
        public string? Summary { get; set; } = string.Empty;

        [MinLength(5, ErrorMessage = "Body must be greater then 5 character.")]
        public string? Body { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public string? Slug { get; set; } = string.Empty;

        public Guid UserID { get; set; }
    }
}
