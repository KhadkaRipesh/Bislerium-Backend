using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Bislerium.Enums;

namespace Domain.Bislerium.Models
{
    [Table("Reaction")]
    public record Reaction
    {
        [Key]
        public Guid ID { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }
        public virtual User? User { get; set; }


        [ForeignKey("Blog")]
        public Guid BlogID { get; set; }

        public virtual Blog? Blog { get; set; }

        [AllowNull]
        [ForeignKey("Comment")]
        public Guid? CommentID { get; set; }

        public virtual Comment? Comment { get; set; }

        public ReactionType Type { get; set; }

        [NotMapped]
        public string TypeName
        {
            get
            {
                return Type.ToString();
            }
            set
            {

            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
