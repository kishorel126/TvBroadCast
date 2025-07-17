using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TvBroadCast.Domain.Interfaces.IAdmin;


namespace TvBroadCast.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllUsersAsync();
            var roles = await _adminService.GetAllRolesAsync();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                userRoles[user.Id] = await _adminService.GetUserRolesAsync(user.Id);
            }

            ViewBag.Roles = roles;
            ViewBag.UserRoles = userRoles;

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            var result = await _adminService.AddUserToRoleAsync(userId, roleName);
            TempData["Message"] = result ? "Role added successfully." : "Failed to add role.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var result = await _adminService.RemoveUserFromRoleAsync(userId, roleName);
            TempData["Message"] = result ? "Role removed successfully." : "Failed to remove role.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            TempData["Message"] = result ? "User deleted successfully." : "Failed to delete user.";
            return RedirectToAction("Index");
        }
    }
}
