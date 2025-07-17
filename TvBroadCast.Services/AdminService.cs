using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvBroadCast.Domain.Interfaces.IAdmin;

namespace TvBroadCast.Services
{
    public class AdminService : IAdminService
    {

        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<IdentityUser>> GetAllUsersAsync()
        {
            return await _adminRepository.GetAllUsersAsync();
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _adminRepository.GetAllRolesAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            return await _adminRepository.GetUserRolesAsync(userId);
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
        {
            return await _adminRepository.AddUserToRoleAsync(userId, roleName);
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            return await _adminRepository.RemoveUserFromRoleAsync(userId, roleName);
        }

        public async Task<IdentityUser?> GetUserByIdAsync(string userId)
        {
            return await _adminRepository.GetUserByIdAsync(userId);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            return await _adminRepository.DeleteUserAsync(userId);
        }

    }
}
