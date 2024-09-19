using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementApp.IServices;
using TaskManagementApp.Models;


namespace TaskManagementApp.Controllers
{
    public class TeamUsersController : Controller
    {
        private readonly ITeamUserService _teamUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        public TeamUsersController(ITeamUserService teamUserService, UserManager<ApplicationUser> userManager)
        {
            _teamUserService = teamUserService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var teamUsers = await _teamUserService.GetTeamUsersAsync(user, User.IsInRole("Admin"));
            return View(teamUsers);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var teamUser = await _teamUserService.GetTeamUserByIdAsync(id.Value);
            if (teamUser == null) return NotFound();

            return View(teamUser);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateViewBags();
            return View(new TeamUsersModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeamId,UserId")] TeamUsersModel teamUsersModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _teamUserService.CreateTeamUsersAsync(teamUsersModel);
                if (result) return RedirectToAction(nameof(Index));

                TempData["TeamExist"] = "Team Already Exists or No Users Selected";
            }

            await PopulateViewBags(teamUsersModel.TeamId, teamUsersModel.UserId);
            return View(teamUsersModel);
        }
        public async Task<IActionResult> Edit(int? id, int? teamId)
        {
            if (id == null) return NotFound();

            var teamUsersModel = await _teamUserService.GetTeamUsersForEditAsync(id.Value, teamId);
            if (teamUsersModel == null) return NotFound();

            await PopulateViewBags(teamUsersModel.TeamId, teamUsersModel.UserId);
            return View(teamUsersModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeamId,UserId")] TeamUsersModel teamUsersModel)
        {
            if (id != teamUsersModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _teamUserService.UpdateTeamUsersAsync(teamUsersModel);
                if (result) return RedirectToAction(nameof(Index));
            }

            await PopulateViewBags(teamUsersModel.TeamId, teamUsersModel.UserId);
            return View(teamUsersModel);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var teamUsersModel = await _teamUserService.GetTeamUsersForDeleteAsync(id.Value);
            if (teamUsersModel == null) return NotFound();

            return View(teamUsersModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _teamUserService.DeleteTeamUsersAsync(id);
            if (!result) return Problem("There was an issue deleting the Team Users.");

            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateViewBags(int? teamId = null, List<string> userId = null)
        {
            ViewBag.TeamId = new SelectList(await _teamUserService.GetTeamsAsync(), "Id", "Name", teamId);
            ViewBag.UserId = new SelectList(await _teamUserService.GetUsersAsync(), "Id", "UserName", userId);
        }
    }
}
