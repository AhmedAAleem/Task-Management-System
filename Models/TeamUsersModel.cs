using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaskManagementApp.Data;

namespace TaskManagementApp.Models
{
    public class TeamUsersModel
    {
        public int Id { get; set; }
        [DisplayName("Team")]
        public int TeamId { get; set; }
        [DisplayName("User")]
        public List<string>? UserId { get; set; }
        [DisplayName("User")]
        public List<ApplicationUser>? Users { get; set; }
        public Team? Team { get; set; }
    }
}
