using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementApp.Data;

namespace TaskManagementApp.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<Comment>> GetAllComments();
        Task<Comment> GetCommentDetails(int? id);
        System.Threading.Tasks.Task CreateComment(Comment comment);
        System.Threading.Tasks.Task UpdateComment(Comment comment);
        System.Threading.Tasks.Task DeleteComment(int id);
        bool CommentExists(int id);
        Task<List<Comment>> GetCommentsByTaskId(int id);

    }
}
