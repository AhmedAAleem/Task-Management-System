using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data;
using TaskManagementApp.IServices;
using TaskManagementApp.Data;
using TaskManagementApp.Models;

namespace TaskManagementApp.Services
{
    public class TeamUserService : ITeamUserService
    {
        private readonly ApplicationDbContext _context;

        public TeamUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeamUsers>> GetTeamUsersAsync(ApplicationUser user, bool isAdmin)
        {
            var teamUsers = await _context.TeamUsers.Include(t => t.Team).Include(t => t.User).ToListAsync();

            var unassignedTeams = await _context.Teams
                .Where(x => !teamUsers.Select(t => t.TeamId).Contains(x.Id))
                .ToListAsync();

            teamUsers.AddRange(unassignedTeams.Select(t => new TeamUsers
            {
                TeamId = t.Id,
                UserId = null,
                Team = t
            }));

            if (!isAdmin)
            {
                teamUsers = teamUsers.Where(x => x.UserId == user.Id).ToList();
            }

            return teamUsers.ToList();
        }

        public async Task<TeamUsersModel> GetTeamUserByIdAsync(int id)
        {
            var teamUser = await _context.TeamUsers
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teamUser == null) return null;

            return new TeamUsersModel
            {
                Id = teamUser.Id,
                Team = teamUser.Team,
                Users = _context.TeamUsers.Where(x => x.TeamId == teamUser.TeamId).Select(x => x.User).ToList()
            };
        }

        public async Task<bool> CreateTeamUsersAsync(TeamUsersModel model)
        {
            if (!_context.TeamUsers.Any(x => x.TeamId == model.TeamId))
            {
                _context.TeamUsers.AddRange(model.UserId.Select(userId => new TeamUsers
                {
                    TeamId = model.TeamId,
                    UserId = userId
                }));
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<TeamUsersModel> GetTeamUsersForEditAsync(int id, int? teamId)
        {
            var teamUsers = await _context.TeamUsers.FindAsync(id) ?? new TeamUsers { TeamId = teamId.Value };

            var users = _context.TeamUsers.Where(x => x.TeamId == teamUsers.TeamId).Select(x => x.UserId).ToList();

            return new TeamUsersModel { Id = id, TeamId = teamUsers.TeamId, UserId = users };
        }

        public async Task<bool> UpdateTeamUsersAsync(TeamUsersModel model)
        {
            var existingUsers = _context.TeamUsers.Where(x => x.TeamId == model.TeamId);
            _context.TeamUsers.RemoveRange(existingUsers);

            _context.TeamUsers.AddRange(model.UserId.Select(userId => new TeamUsers
            {
                TeamId = model.TeamId,
                UserId = userId
            }));

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TeamUsersModel> GetTeamUsersForDeleteAsync(int id)
        {
            var teamUser = await _context.TeamUsers
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teamUser == null) return null;

            return new TeamUsersModel
            {
                Id = id,
                Team = teamUser.Team,
                Users = _context.TeamUsers.Where(x => x.TeamId == teamUser.TeamId).Select(x => x.User).ToList()
            };
        }

        public async Task<bool> DeleteTeamUsersAsync(int id)
        {
            var teamUsers = await _context.TeamUsers.FindAsync(id);
            if (teamUsers != null)
            {
                _context.TeamUsers.RemoveRange(_context.TeamUsers.Where(x => x.TeamId == teamUsers.TeamId));
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Team>> GetTeamsAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
