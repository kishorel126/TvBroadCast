using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IApproval;
using TvBroadCast.Domain.Interfaces.IBroadCast;
using TvBroadCast.Services;

namespace TvBroadCast.Web.Controllers
{

    [Authorize]
    public class BroadcastController : Controller
    {

        private readonly IBroadCastService _broadCastService;
        private readonly IApprovalService _approvalService;

        public BroadcastController(IBroadCastService broadCastService, IApprovalService approvalService)
        {
            _broadCastService = broadCastService;
            _approvalService = approvalService;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddHours(-2); // 2 hours before current time
            var windowEnd = now.AddHours(2.5); // 2 hours after current time

            var broadcasts = await _broadCastService.GetBroadCastForTimeWindowAsync(windowStart, windowEnd);

            ViewBag.WindowStart = windowStart;
            ViewBag.WindowEnd = windowEnd;

            return View(broadcasts);

        }

        // AJAX endpoint : Get broadcast for time window
        [HttpGet]
        public async Task<IActionResult> GetSchedule(DateTime windowStart, DateTime windowEnd)
        {
            var broadcasts = await _broadCastService.GetBroadCastForTimeWindowAsync(windowStart, windowEnd);
            return PartialView("_ScheduleTable", broadcasts);
        }

        //Scheduler: Add broadcast (AJAX)
        [Authorize(Roles = "Scheduler")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BroadCast model)
        {
            model.SchedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _broadCastService.AddBroadCastAsync(model);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }

        // Scheduler: Edit broadcast (AJAX)
        [Authorize(Roles = "Scheduler")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] BroadCast model)
        {
            model.SchedulerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _broadCastService.UpdateBroadCastAsync(model);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }

        // Scheduler: Delete broadcast (AJAX)
        [Authorize(Roles = "Scheduler")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var result = await _broadCastService.DeleteBroadCastAsync(id);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = "Failed to delete broadcast." });
        }

        // Approver: Approve broadcast (AJAX)
        [Authorize(Roles = "Approver")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Approve([FromBody] ApprovalRequestDto dto)
        {
            var approverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _approvalService.ApproveBroadCastAsync(dto.BroadcastId, approverId, dto.Comment);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }

        // Approver: Reject broadcast (AJAX)
        [Authorize(Roles = "Approver")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Reject([FromBody] ApprovalRequestDto dto)
        {
            var approverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _approvalService.RejectBroadCastAsync(dto.BroadcastId, approverId, dto.Comment);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }

        // Get approval history (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetHistory(int broadcastId)
        {
            var history = await _approvalService.GetApprovalHistoryForBroadCastAsync(broadcastId);
            return PartialView("_ApprovalHistory", history);
        }

    }

}

// DTO for approval requests
public class ApprovalRequestDto
{
    public int BroadcastId { get; set; }
    public string Comment { get; set; }
}
