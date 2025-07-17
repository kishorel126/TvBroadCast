
using Microsoft.EntityFrameworkCore;
using TvBroadCast.DataAccess.DbContext;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IApproval;

namespace TvBroadCast.DataAccess.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {

        private readonly AppDbContext _context;

        public ApprovalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApprovalHistory?> GetLatestForBroadcastAsync(int broadcastId)
        {
            return await _context.ApprovalHistories
                .Where(a => a.BroadcastId == broadcastId)
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ApprovalHistory>> GetHistoryForBroadcastAsync(int broadcastId)
        {
            return await _context.ApprovalHistories
                .Where(h => h.BroadcastId == broadcastId)
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();
        }

        public async Task AddApprovalHistoryAsync(ApprovalHistory approvalHistory)
        {

            _context.ApprovalHistories.Add(approvalHistory);
            await _context.SaveChangesAsync();

        }

    }
}
