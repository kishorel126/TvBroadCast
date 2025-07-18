using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TvBroadCast.Domain.Interfaces.IBroadCast;

namespace TvBroadCast.Web.Controllers
{
    [Authorize]
    public class BroadcastController : Controller
    {
        private readonly IBroadCastService _broadCastService;

        public BroadcastController(IBroadCastService broadCastService)
        {
            _broadCastService = broadCastService;
        }

        // View schedule (open to all)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var windowStart = now.AddHours(-2);
            var windowEnd = now.AddHours(2.5);
            var broadcasts = await _broadCastService.GetBroadCastForTimeWindowAsync(windowStart, windowEnd);
            ViewBag.WindowStart = windowStart;
            ViewBag.WindowEnd = windowEnd;
            return View(broadcasts);
        }

        // AJAX: Get broadcasts for a given time window (all roles)
        [HttpGet]
        public async Task<IActionResult> GetSchedule(DateTime windowStart, DateTime windowEnd)
        {
            ViewBag.windowStart = windowStart;
            ViewBag.windowEnd = windowEnd;
            var broadcasts = await _broadCastService.GetBroadCastForTimeWindowAsync(windowStart, windowEnd);
            return PartialView("_ScheduleTable", broadcasts);
        }
    }
}