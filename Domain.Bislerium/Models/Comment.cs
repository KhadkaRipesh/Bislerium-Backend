using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    [Table("Comment")]
    public record Comment
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Body should be greater than 1 characters")]
        [MaxLength(225, ErrorMessage = "Damage Request Message Must Be Less Then 225 Character Long.")]
        public string Body { get; set; } = string.Empty;

        // User foreign Key
        [ForeignKey("User")]
        public Guid UserID { get; set; }

        public virtual User? User { get; set; }

        [ForeignKey("Blog")]
        public Guid BlogID { get; set; }

        public virtual Blog? Blog { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

    }
}
