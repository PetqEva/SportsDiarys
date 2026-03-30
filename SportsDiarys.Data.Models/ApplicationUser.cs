using Microsoft.AspNetCore.Identity;

namespace SportsDiarys.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserProfile? UserProfile { get; set; }
    }
}

