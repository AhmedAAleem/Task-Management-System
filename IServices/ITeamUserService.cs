using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementApp.Models;
using Microsoft.AspNetCore.Identity;
using TaskManagementApp.Data;

namespace TaskManagementApp.IServices
{
    public interface ITeamUserService
    {
        Task<List<TeamUsers>> GetTeamUsersAsync(ApplicationUser user, bool isAdmin);
        Task<TeamUsersModel> GetTeamUserByIdAsync(int id);
        Task<bool> CreateTeamUsersAsync(TeamUsersModel model);
        Task<TeamUsersModel> GetTeamUsersForEditAsync(int id, int? teamId);
        Task<bool> UpdateTeamUsersAsync(TeamUsersModel model);
        Task<TeamUsersModel> GetTeamUsersForDeleteAsync(int id);
        Task<bool> DeleteTeamUsersAsync(int id);
        Task<List<Team>> GetTeamsAsync();
        Task<List<ApplicationUser>> GetUsersAsync();
    }
}
