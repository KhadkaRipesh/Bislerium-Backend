using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Domain.Bislerium.Enums;

namespace Domain.Bislerium.Models
{
    [Table("User")]
    public record User
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be greater than 2 character")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter password.")]
        [MinLength(5, ErrorMessage = "Password must be greator then 5 character.")]
        public string Password { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.BLOGGER;

        [NotMapped]
        public string AccessToken { get; set; } = string.Empty;

        [NotMapped]
        public string RoleName
        {
            get
            {
                return Role.ToString();
            }
            set
            {

            }
        }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;


        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; }

        [JsonIgnore]
        public ICollection<Reaction> Reactions { get; set; }



    }
}
