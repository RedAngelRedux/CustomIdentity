using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace CustomIdentity.Models
{
    public class CustomUser : IdentityUser
    {
        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public int UsernameChangeLimit { get; set; } = 10;

        [Display(Name ="Profile Picture")]
        public byte[]? ProfileImageData { get; set; }

        [Display(Name = "Profile Picture Type")]
        public string? ProfileImageType { get; set; }

        [NotMapped]
        public IFormFile? ProfileImageFile { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        //// Parent of...
        //public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
        //public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();


    }
}
