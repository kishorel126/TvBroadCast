using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IApproval;
using TvBroadCast.Domain.Interfaces.IBroadCast;

namespace TvBroadCast.Web.Controllers
{
    [Authorize(Roles = "Approver,Admin")]
    public class ApproverController : Controller
    {
        private readonly IApprovalService _approvalService;
        private readonly IBroadCastService _broadCastService;

        public ApproverController(IApprovalService approvalService, IBroadCastService broadCastService)
        {
            _approvalService = approvalService;
            _broadCastService = broadCastService;
        }

        // List all broadcasts (you can filter as needed)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var broadcasts = (await _broadCastService.GetAllAsync());
            return View(broadcasts);
        }

        // REVIEW PAGE (GET)
        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var broadcast = await _broadCastService.GetBroadCastByIdAsync(id);
            if (broadcast == null)
                return NotFound();
            return View("Review", broadcast);
        }

        // AJAX: Approve a broadcast
        [HttpPost]
        public async Task<IActionResult> Approve([FromBody] ApprovalRequestDto dto)
        {
            var approverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _approvalService.ApproveBroadCastAsync(dto.BroadcastId, approverId, dto.Comment);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }

        // AJAX: Reject a broadcast
        [HttpPost]
        public async Task<IActionResult> Reject([FromBody] ApprovalRequestDto dto)
        {
            var approverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _approvalService.RejectBroadCastAsync(dto.BroadcastId, approverId, dto.Comment);
            if (result.Success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, error = result.Error });
        }

        // AJAX: Get approval history
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetHistory(int broadcastId)
        {
            var history = await _approvalService.GetApprovalHistoryForBroadCastAsync(broadcastId);
            return PartialView("_ApprovalHistory", history);
        }
    }

    // DTO for approval actions
    public class ApprovalRequestDto
    {
        public int BroadcastId { get; set; }
        public string Comment { get; set; }
    }
}