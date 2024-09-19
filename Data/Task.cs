using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using TaskManagementApp.Models;
using static TaskManagementApp.Models.Enums;

namespace TaskManagementApp.Data
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority? Priority { get; set; }
        public Status? Status { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public int? DepentTaskId { get; set; }
        public List<Comment>? Comments { get; set;}

    }
}