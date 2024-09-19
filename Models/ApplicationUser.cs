using Microsoft.AspNetCore.Identity;
using TaskManagementApp.Data;

namespace TaskManagementApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Comment>? Comments { get; set; }
        public string? RoleId { get; set; }

    }
}
