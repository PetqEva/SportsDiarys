using Microsoft.AspNetCore.Identity;
using SportsDiarys.Data.Models;

namespace SportDiary.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserProfile? UserProfile { get; set; }
    }
}

