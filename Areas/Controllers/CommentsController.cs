using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagementApp.Data;
using TaskManagementApp.Services.Interfaces;

namespace TaskManagementApp.Areas.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ApplicationDbContext _context;

        public CommentsController(ICommentService commentService, ApplicationDbContext context)
        {
            _commentService = commentService;
            _context = context;
        }

 

        public async Task<IActionResult> Index(int id)
        {
            var comments = await _commentService.GetCommentsByTaskId(id);
            ViewBag.taskId = id;
            return View(comments);
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var comment = await _commentService.GetCommentDetails(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create(int id)
        {
            ViewBag.tasks = new SelectList(_context.Tasks, "Id", "Title",id);
            ViewBag.users = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.taskId = id.ToString();
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,CreatedAt,UserId,TaskId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Id = 0;
                comment.CreatedAt = DateTime.Now;
                await _commentService.CreateComment(comment);
                return RedirectToAction(nameof(Index),new {Id= comment.TaskId});
            }
            ViewData["tasks"] = new SelectList(_context.Tasks, "Id", "Title", comment.TaskId);
            ViewData["users"] = new SelectList(_context.Users, "Id", "UserName", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var comment = await _commentService.GetCommentDetails(id);
            if (comment == null)
            {
                return NotFound();
            }

            ViewData["tasks"] = new SelectList(_context.Tasks, "Id", "Title", comment.TaskId);
           
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,CreatedAt,UserId,TaskId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _commentService.UpdateComment(comment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_commentService.CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { Id = comment.TaskId });
            }

            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Id", comment.TaskId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var comment = await _commentService.GetCommentDetails(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int taskId)
        {
            await _commentService.DeleteComment(id);
            return RedirectToAction(nameof(Index), new { Id = taskId });
        }
    }
}
