using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TvBroadCast.Domain.Interfaces.IAdmin;
using Microsoft.AspNetCore.SignalR;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Web.Hubs;


namespace TvBroadCast.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly IAdminService _adminService;
        private readonly IHubContext<BroadcastHub> _hubContext;

        public AdminController(IAdminService adminService , IHubContext<BroadcastHub> hubContext)
        {
            _adminService = adminService;
            _hubContext = hubContext;
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
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var result = await _adminService.RemoveUserFromRoleAsync(userId, roleName);
            TempData["Message"] = result ? "Role removed successfully." : "Failed to remove role.";
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _adminService.DeleteUserAsync(userId);
            TempData["Message"] = result ? "User deleted successfully." : "Failed to delete user.";
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
            return RedirectToAction("Index");
        }
    }
}
