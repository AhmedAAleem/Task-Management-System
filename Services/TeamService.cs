using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data;
using TaskManagementApp.IServices;
using TaskManagementApp.Models;

namespace TaskManagementApp.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;
        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Team>> GetTeamsAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            return await _context.Teams.Include(s=>s.TeamUsers).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async System.Threading.Tasks.Task CreateTeamAsync(Team team)
        {
            var createdTeam =_context.Teams.Add(team);
            await _context.SaveChangesAsync();
            foreach (var userId in team.UserIds)
            {
                _context.TeamUsers.Add(new TeamUsers { TeamId = createdTeam.Entity.Id, UserId = userId });

            }
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateTeamAsync(Team team)
        {
            if (team.UserIds.Count>0)
            {
                var currentTeamUsers = await _context.TeamUsers.Where(t => t.TeamId == team.Id).ToListAsync();
                _context.TeamUsers.RemoveRange(currentTeamUsers);

               await _context.TeamUsers.AddRangeAsync(team.UserIds.Select(s=>new TeamUsers {TeamId=team.Id,UserId=s}));

            }
            _context.Update(team);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TeamExistsAsync(team.Id))
                {
                    return false;
                }
                throw;
            }
        }
        public async Task<(bool status,string msg)> DeleteTeamAsync(int id)
        {
            var team = await GetTeamByIdAsync(id);
            if (team == null) return (false,"");

            if(_context.Tasks.Any(x => x.TeamId == team.Id)) { return (false,"Please handle pending tasks first!"); }

            _context.TeamUsers.RemoveRange(_context.TeamUsers.Where(x => x.TeamId == team.Id));
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return (true,"");
        }
        private async Task<bool> TeamExistsAsync(int id)
        {
            return await _context.Teams.AnyAsync(e => e.Id == id);
        }
    }
}
