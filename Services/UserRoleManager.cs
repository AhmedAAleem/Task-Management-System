using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data;
using TaskManagementApp.Models;

namespace TaskManagementApp.Services
{
    public class UserRoleManager
    {
        private ApplicationDbContext ctx { get; set; }
        private RoleManager<IdentityRole> roleManager { get; set; }
        public UserRoleManager(ApplicationDbContext ctx, RoleManager<IdentityRole> roleManager)
        {
            this.ctx = ctx;
            this.roleManager = roleManager;
        }
        public async Task<(string RoleId,string RoleName)> GetRoleByUserId(string userId)
        {
            var userrole = await ctx.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
            var roleName = (await roleManager.FindByIdAsync(userrole?.RoleId)).Name;
            if (userrole != null && roleName != null)
            {
                return (userrole.RoleId, roleName);
            }
            return (null, null);
        }
        public async Task<IdentityUserRole<string>> FindByUserIdAsync(string userId)
        {
            return await ctx.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task<bool> DeletebyuseridAsync(string userId)
        {
            try
            {
                ctx.UserRoles.Remove(await FindByUserIdAsync(userId));
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

