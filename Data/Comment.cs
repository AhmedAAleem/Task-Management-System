using TaskManagementApp.Models;

namespace TaskManagementApp.Data
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int? TaskId { get; set; }
        public virtual Task? Task { get; set; }
    }
}
