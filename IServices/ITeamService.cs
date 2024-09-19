using TaskManagementApp.Data;
using static TaskManagementApp.Models.Enums;

namespace TaskManagementApp.IServices
{
    public interface ITeamService
    {
        Task<List<Team>> GetTeamsAsync();
        Task<Team> GetTeamByIdAsync(int id);
        System.Threading.Tasks.Task CreateTeamAsync(Team team);
        Task<bool> UpdateTeamAsync(Team team);
        Task<(bool status, string msg)> DeleteTeamAsync(int id);
    }
}
