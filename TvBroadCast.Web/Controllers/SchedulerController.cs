using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index()
        {
            var schedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var broadcasts = (await _broadCastService.GetAllAsync()).Where(b => b.SchedulerId == schedulerId);

            return View(broadcasts);
        }

        public IActionResult Create()
        {
            return View();
        }
        // AJAX: Add a broadcast
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BroadCast model)
        {
            model.SchedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _broadCastService.AddBroadCastAsync(model);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }

        public IActionResult Edit()
        {
            return View();
        }
        // AJAX: Edit a broadcast
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] BroadCast model)
        {
            model.SchedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _broadCastService.UpdateBroadCastAsync(model);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }


        public IActionResult Delete()
        {
            return View();
        }

        // AJAX: Delete a broadcast
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var result = await _broadCastService.DeleteBroadCastAsync(id);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = "Failed to delete broadcast." });
        }
    }
}