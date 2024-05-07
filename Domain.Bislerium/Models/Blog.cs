using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    [Table("Blog")]
    public record Blog
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Title should be greater than 8 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Summary should be greater than 8 characters")]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Body must be greater then 8 character.")]
        public string Body { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        // User foreign Key
        [ForeignKey("User")]
        public Guid UserID { get; set; }

        public virtual User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; }

        [JsonIgnore]
        public ICollection<Reaction> Reactions { get; set; }

    }
}
