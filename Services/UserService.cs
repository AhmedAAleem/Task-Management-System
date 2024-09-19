using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementApp.Data;
using TaskManagementApp.IServices;
using TaskManagementApp.Models;



namespace TaskManagementApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(ApplicationDbContext context,
                           UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetUserForEditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.NormalizedEmail = _context.UserRoles.FirstOrDefault(x => x.UserId == id)?.RoleId;
            }
            return user;
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> EditUserAsync(string id, IFormCollection collection)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var roleId = collection["RoleId"].ToString();
                var newRole = _roleManager.Roles.FirstOrDefault(r=>r.Id== roleId);

                // Remove old role(s)
                foreach (var role in currentRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
                if (newRole != null)
                {
                    // Add new role
                    if (await _roleManager.RoleExistsAsync(newRole.Name))
                    {
                        await _userManager.AddToRoleAsync(user, newRole.Name);
                    }

                }
                // Update username
                user.UserName = collection["UserName"].ToString();
                user.RoleId = roleId;
                await _userManager.UpdateAsync(user);

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return true;
            }
            return false;
        }

        public async Task<bool> IsCurrentUserAsync(string currentUserName, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName == currentUserName;
        }
    }
}
