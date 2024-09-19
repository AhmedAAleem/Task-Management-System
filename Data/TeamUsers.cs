using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementApp.Models;

namespace TaskManagementApp.Data
{
    public class TeamUsers
    {
        public int Id { get; set; }
        [ForeignKey("Team")]
        [DisplayName("Team")]
        public int TeamId { get; set; }
        [ForeignKey("User")]
        [DisplayName("User")]
        public string UserId { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ApplicationUser? User { get; set; }

    }
}
