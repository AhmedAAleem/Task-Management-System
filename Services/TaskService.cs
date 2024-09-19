using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementApp.Data;
using TaskManagementApp.Models;
using TaskManagementApp.Services.Interfaces;
using static TaskManagementApp.Models.Enums;

namespace TaskManagementApp.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<TaskSearchModel> GetTaskSearchForIndex(string userName, bool isAdmin, bool isTeamLeader, bool isUser)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var statusList = Enum.GetValues<Status>()
                .Select(x => new SelectListItem { Value = ((int)x).ToString(), Text = x.ToString() })
                .ToList();
            statusList.Insert(0, new SelectListItem { Value = null, Text = "Choose", Disabled = true, Selected = true });

            var userTeams = await _context.Teams.Include(x => x.TeamUsers).ToListAsync();
            userTeams = isAdmin ? userTeams : userTeams.Where(x => x.TeamUsers.Select(xx => xx.UserId).Contains(user.Id)).ToList();

            var teamslist = userTeams.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            var tasks = await _context.Tasks.Include(t => t.Team).Include(t => t.User).ToListAsync();

            if (isTeamLeader)
            {
                tasks = tasks.Where(x => teamslist.Select(xx => Convert.ToInt32(xx.Value)).Contains(x.TeamId ?? 0)).ToList();
            }
            else if (isUser)
            {
                tasks = tasks.Where(x => x.UserId == user.Id).ToList();
            }

            teamslist.Insert(0, new SelectListItem { Value = null, Text = "Choose", Disabled = true, Selected = true });

            return new TaskSearchModel
            {
                Tasks = tasks,
                statusList = statusList,
                TeamsList = teamslist
            };
        }

        public async Task<TaskManagementApp.Data.Task> GetTaskDetails(int? id)
        {
            if (id == null) return null;

            return await _context.Tasks
                                 .Include(t => t.Team)
                                 .Include(t => t.User)
                                 .Include(t => t.Comments)
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async System.Threading.Tasks.Task CreateTask(TaskManagementApp.Data.Task task)
        {
            await _context.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task UpdateTask(TaskManagementApp.Data.Task task)
        {
            _context.Update(task);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                var subtasks = _context.Tasks.Where(x => x.DepentTaskId == id);
                var comments = _context.Comments.Where(x => x.TaskId == id);
                _context.Tasks.RemoveRange(subtasks);
                _context.Comments.RemoveRange(comments);
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
