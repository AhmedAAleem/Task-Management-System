using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementApp.Models;

namespace TaskManagementApp.Data
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public List<string> UserIds { get; set; }
        public ICollection<TeamUsers>? TeamUsers { get; set; }
        public ICollection<Task>? Tasks { get; set; }
    }
}