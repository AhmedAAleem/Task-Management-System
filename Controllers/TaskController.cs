using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagementApp.Data;
using TaskManagementApp.Models;
using TaskManagementApp.Services;
using TaskManagementApp.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static TaskManagementApp.Models.Enums;

namespace TaskManagementApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public TasksController(ITaskService taskService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _taskService = taskService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var searchModel = await _taskService.GetTaskSearchForIndex(User.Identity.Name,
                User.IsInRole("Admin"), User.IsInRole("TeamLeader"), User.IsInRole("User"));
            return View(searchModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("statusId,TeamId,title,statusList,TeamsList")] TaskSearchModel search)
        {
            var searchModel = await _taskService.GetTaskSearchForIndex(User.Identity.Name,
                User.IsInRole("Admin"), User.IsInRole("TeamLeader"), User.IsInRole("User"));

            if (search.statusId != null)
            {
                searchModel.Tasks = searchModel.Tasks.Where(x => (int)x.Status == search.statusId).ToList();
            }
            if (search.TeamId != null)
            {
                searchModel.Tasks = searchModel.Tasks.Where(x => x.TeamId == search.TeamId).ToList();
            }
            if (!string.IsNullOrEmpty(search.title))
            {
                searchModel.Tasks = searchModel.Tasks.Where(x => x.Title.Contains(search.title)).ToList();
            }

            return View(searchModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var task = await _taskService.GetTaskDetails(id);
            if (task == null)
            {
                return NotFound();
            }
            foreach (var comment in task?.Comments)
            {

                var userName = comment.User.UserName;

                var length = userName.LastIndexOf("@");

                if (length != -1)
                    comment.User.UserName = userName.Substring(0, length);
            }
            ViewBag.Subtasks = task?.DepentTaskId == null ? _context.Tasks.Where(x => x.DepentTaskId == task.Id)
                                                          :_context.Tasks.Where(x => x.Id == task.DepentTaskId);
             
            ViewBag.SubTaskName = task?.DepentTaskId == null ? "SubTasks" : "Parent Task";

            ViewBag.Comments = task?.Comments;
            ViewBag.CommentsName = "Comments";


            return View(task);
        }

        public async Task<IActionResult> Create(int? depndentId)
        {
            var PriorityList = Enum.GetValues<Priority>().Select(x => new { Id = (int)x, Name = x.ToString() }).OrderByDescending(x => x.Id).AsEnumerable();
            ViewData["PriorityList"] = new SelectList(PriorityList, "Id", "Name");
            var statusList = Enum.GetValues<Status>().Select(x => new { Id = (int)x, Name = x.ToString() });
            ViewData["StatusList"] = new SelectList(statusList, "Id", "Name");


            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();

            teamList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Select",
                Selected = true
            });
            ViewData["TeamId"] = new SelectList(teamList, "Value", "Text");

            var userList = _context.Users.Select(t => new SelectListItem
            {
                Value = t.Id,
                Text = t.UserName
            }).ToList();

            userList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Select",
                Selected = true
            });
            ViewData["UserId"] = new SelectList(userList, "Value", "Text");

            return View(new Data.Task()
            {
                UserId = (await _userManager.FindByNameAsync(User.Identity?.Name)).Id,
                DepentTaskId = depndentId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,status,TeamId,UserId,DepentTaskId,Priority")] Data.Task task)
        {
            if (ModelState.IsValid)
            {
                await _taskService.CreateTask(task);
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var task = await _taskService.GetTaskDetails(id);
            if (task == null)
            {
                return NotFound();
            }
            var PriorityList = Enum.GetValues<Priority>().Select(x => new { Id = (int)x, Name = x.ToString() }).OrderByDescending(x => x.Id);
            ViewData["PriorityList"] = new SelectList(PriorityList, "Id", "Name");
            var statusList = Enum.GetValues<Status>().Select(x => new { Id = (int)x, Name = x.ToString() });
            ViewData["StatusList"] = new SelectList(statusList, "Id", "Name");

            var teamList = _context.Teams.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name, 
                Selected= t.Id == task.TeamId
            }).ToList();

            teamList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Select"
            });
            ViewData["TeamId"] = new SelectList(teamList, "Value", "Text");

            var userList = _context.Users.Select(t => new SelectListItem
            {
                Value = t.Id,
                Text = t.UserName,
                Selected= t.Id == task.UserId
            }).ToList();

            userList.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "Select"
                
            });
            ViewData["UserId"] = new SelectList(userList, "Value", "Text");
            ViewBag.TeamsUsers = _context.TeamUsers.ToList();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,TeamId,UserId,DepentTaskId,Priority")] Data.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _taskService.UpdateTask(task);
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var task = await _taskService.GetTaskDetails(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _taskService.DeleteTask(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
