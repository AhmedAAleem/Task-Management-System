using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementApp.IServices;
using TaskManagementApp.Models;

namespace TaskManagementApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserController(IUserService userService,
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return users != null ? View(users) : Problem("Entity set is null.");
        }

        public async Task<ActionResult> EditAsync(string id)
        {
            var user = await _userService.GetUserForEditAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = await _userService.GetAllRolesAsync();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(string id, IFormCollection collection)
        {
            try
            {
                var result = await _userService.EditUserAsync(id, collection);

                if (result && await _userService.IsCurrentUserAsync(User.Identity?.Name, id))
                {
                    await _signInManager.SignOutAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (result && await _userService.IsCurrentUserAsync(User.Identity?.Name, id))
                {
                    await _signInManager.SignOutAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                var user = await _userService.GetUserByIdAsync(id);
                return View(user);
            }
        }
    }
}
