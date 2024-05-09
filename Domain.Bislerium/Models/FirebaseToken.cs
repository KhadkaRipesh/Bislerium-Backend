using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Models
{
    [Table("FirebaseToken")]
    public record FirebaseToken
    {
        [Key]
        public Guid ID { get; set; }

        // User foreign Key
        [ForeignKey("User")]
        public Guid UserID { get; set; }
        public virtual User? User { get; set; }

        public string? Token {  get; set; }

    }
}
