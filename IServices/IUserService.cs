using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementApp.Models;

namespace TaskManagementApp.IServices
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<ApplicationUser> GetUserForEditAsync(string id);
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task<bool> EditUserAsync(string id, IFormCollection collection);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> IsCurrentUserAsync(string currentUserName, string userId);
    }
}
