using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementApp.Models;

namespace TaskManagementApp.Services.Interfaces
{
    public interface ITaskService
    {
        Task<TaskSearchModel> GetTaskSearchForIndex(string userName, bool isAdmin, bool isTeamLeader, bool isUser);
        Task<TaskManagementApp.Data.Task> GetTaskDetails(int? id);
        Task CreateTask(TaskManagementApp.Data.Task task);
        Task UpdateTask(TaskManagementApp.Data.Task task);
        Task DeleteTask(int id);
        bool TaskExists(int id);
    }
}
