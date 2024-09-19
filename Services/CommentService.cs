using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementApp.Data;
using TaskManagementApp.Services.Interfaces;

namespace TaskManagementApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAllComments()
        {
            return await _context.Comments.Include(c => c.Task).Include(c => c.User).ToListAsync();
        }
        public async Task<Comment> GetCommentDetails(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return await _context.Comments
                                 .Include(c => c.Task)
                                 .Include(c => c.User)
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async System.Threading.Tasks.Task CreateComment(Comment comment)
        {
            _context.Add(comment);
            await _context.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task UpdateComment(Comment comment)
        {
            _context.Update(comment);
            await _context.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
        public bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
        public async Task<List<Comment>> GetCommentsByTaskId(int id)
        {
            return await _context.Comments.Where(x => x.TaskId == id).Include(t=>t.Task).ToListAsync();
        }
    }
}
