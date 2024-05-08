using Domain.Bislerium.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.DTOs.Reaction
{
    public record CreateReaction
    {
        [Required]
        public Guid UserID { get; set; }

        [Required]
        public Guid BlogID { get; set; }

        [AllowNull]
        public Guid? CommentID { get; set; }

        public ReactionType Type { get; set; }
    }
}
