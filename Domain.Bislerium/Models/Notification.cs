using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    [Table("Notification")]
    public record Notification
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;

        // Foreign Key from user
        [ForeignKey("User")]
        public Guid UserID { get; set; }

        public virtual User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
