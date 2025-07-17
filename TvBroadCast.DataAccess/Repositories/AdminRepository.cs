
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IAdmin;

namespace TvBroadCast.DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<IdentityUser>> GetAllUsersAsync()
        {
            var users =  _userManager.Users.ToList();

            return users;
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return  _roleManager.Roles.ToList();
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || !await _roleManager.RoleExistsAsync(roleName))
                return false;
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _roleManager.RoleExistsAsync(roleName))
                return false;
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<IdentityUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

    }
}
