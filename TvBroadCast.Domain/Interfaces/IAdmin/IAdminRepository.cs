using Microsoft.AspNetCore.Identity;

namespace TvBroadCast.Domain.Interfaces.IAdmin
{
    public interface IAdminRepository
    {

        Task<List<IdentityUser>> GetAllUsersAsync();
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<bool> AddUserToRoleAsync(string userId, string roleName);
        Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<IdentityUser?> GetUserByIdAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);


    }
}
