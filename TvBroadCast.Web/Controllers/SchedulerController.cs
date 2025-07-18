using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IBroadCast;

namespace TvBroadCast.Web.Controllers
{
    [Authorize(Roles = "Scheduler,Admin")]
    public class SchedulerController : Controller
    {
        private readonly IBroadCastService _broadCastService;

        public SchedulerController(IBroadCastService broadCastService)
        {
            _broadCastService = broadCastService;
        }

        // Loads the scheduler dashboard view (table and buttons only, no model)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Loads the create form view (empty form)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new BroadCast());
        }

        // Loads the edit form view (form pre-populated by AJAX)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var broadcast = await _broadCastService.GetBroadCastByIdAsync(id);
            if (broadcast == null)
                return NotFound();

            return View(broadcast);
        }

        // Returns all broadcasts for the current scheduler as JSON (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetMyBroadcasts()
        {
            var schedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var broadcasts = (await _broadCastService.GetAllAsync())
                .Where(b => b.SchedulerId == schedulerId)
                .OrderByDescending(b => b.StartTime)
                .ToList();
            return Json(new { success = true, data = broadcasts });
        }

        // Creates a broadcast via AJAX (returns JSON)
        [HttpPost]
        public async Task<IActionResult> CreateBroadcast([FromBody] BroadCast model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
            }

            model.SchedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _broadCastService.AddBroadCastAsync(model);

            if (result.Success)
                return Json(new { success = true, message = "Broadcast added successfully." });

            return Json(new { success = false, error = result.Error });
        }

        // Updates a broadcast via AJAX (returns JSON)
        [HttpPost]
        public async Task<IActionResult> EditBroadcast([FromBody] BroadCast model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
            }

            model.SchedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _broadCastService.UpdateBroadCastAsync(model);

            if (result.Success)
                return Json(new { success = true, message = "Broadcast updated successfully." });

            return Json(new { success = false, error = result.Error });
        }

        // Deletes a broadcast via AJAX (returns JSON)
        [HttpPost]
        public async Task<IActionResult> DeleteBroadcast([FromBody] int id)
        {
            var result = await _broadCastService.DeleteBroadCastAsync(id);
            if (result.Success)
                return Json(new { success = true, message = "Broadcast deleted successfully." });

            return Json(new { success = false, error = "Failed to delete broadcast." });
        }
    }
}