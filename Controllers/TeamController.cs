using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Models;
using TaskManagementApp.Data;
using TaskManagementApp.IServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Services.Interfaces;
using TaskManagementApp.Services;

namespace TaskManagementApp.Controllers
{
    [Authorize(Roles = "Admin,TeamLead")]
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly ApplicationDbContext _context;
      
        public TeamsController(ITeamService teamService, ApplicationDbContext context)
        {
            _teamService = teamService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetTeamsAsync();
            return View(teams);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var team = await _teamService.GetTeamByIdAsync(id.Value);
            if (team == null) return NotFound();

            return View(team);
        }

        public IActionResult Create()
        {

            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)       
        {
            if (ModelState.IsValid)
            {
                await _teamService.CreateTeamAsync(team);
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var team = await _teamService.GetTeamByIdAsync(id.Value);

            if (team == null) return NotFound();

            team.UserIds = team.TeamUsers.Select(tu => tu.UserId).ToList();

            var allUsers = _context.Users.ToList().Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName
            }).ToList();

            ViewBag.Users = new SelectList(allUsers, "Value", "Text");

            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Team team)
        {
            if (id != team.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _teamService.UpdateTeamAsync(team);
                if (result) return RedirectToAction(nameof(Index));
            }
            return View(team);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var team = await _teamService.GetTeamByIdAsync(id.Value);
            if (team == null) return NotFound();

            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _teamService.DeleteTeamAsync(id);
            if (!result.status) return Problem(result.msg==""?"There was a problem deleting the team.": result.msg);

            return RedirectToAction(nameof(Index));
        }
    }
}
