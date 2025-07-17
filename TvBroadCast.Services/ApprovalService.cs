
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IApproval;
using TvBroadCast.Domain.Interfaces.IBroadCast;

namespace TvBroadCast.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IBroadCastRepository _broadcastRepository;

        public ApprovalService(IApprovalRepository approvalRepository , IBroadCastRepository broadCastRepository)
        {
            _approvalRepository = approvalRepository;
            _broadcastRepository = broadCastRepository;
        }

        public async Task<(bool Success , string Error)> ApproveBroadCastAsync(int broadcastId , string approverId , string comment)
        {

            var broadcast = await _broadcastRepository.GetBroadCastByIdAsync(broadcastId);
            if(broadcast == null)
            {
                return (false , "Broadcast not found!");
            }

            if(broadcast.Status != BroadCastStatus.BStatus.Pending)
            {
                return (false , "Broadcast is not pending approval!");
            }

            //Optional : checking for overlaps again before approval
            if(await _broadcastRepository.IsOverlappingAsync(broadcast.StartTime , broadcast.EndTime , broadcastId))
            {
                return (false , "Broadcast time overlaps with existing schedule");
            }

            broadcast.Status = BroadCastStatus.BStatus.Approved;
            broadcast.ApproverId = approverId;
            broadcast.ApproverComment = comment;

            await _broadcastRepository.UpdateAsync(broadcast);

            var history = new ApprovalHistory
            {
                BroadcastId = broadcastId,
                ApproverId = approverId,
                Timestamp = DateTime.UtcNow,
                Status = BroadCastStatus.BStatus.Approved,
                Comment = comment
            };

            await _approvalRepository.AddApprovalHistoryAsync(history);

            return (true , "Broadcast approved successfully!");
        }

        public async Task<(bool Success , string Error)> RejectBroadCastAsync(int broadcastId , string approverId , string comment)
        {
            var broadcast = await _broadcastRepository.GetBroadCastByIdAsync(broadcastId);
            if (broadcast == null)
                return (false, "Broadcast not found.");

            if (broadcast.Status != BroadCastStatus.BStatus.Pending)
                return (false, "Only pending broadcasts can be rejected.");

            broadcast.Status = BroadCastStatus.BStatus.Rejected;
            broadcast.ApproverId = approverId;
            broadcast.ApproverComment = comment;

            await _broadcastRepository.UpdateAsync(broadcast);

            var history = new ApprovalHistory
            {
                BroadcastId = broadcast.Id,
                ApproverId = approverId,
                Timestamp = DateTime.UtcNow,
                Status = BroadCastStatus.BStatus.Rejected,
                Comment = comment
            };
            await _approvalRepository.AddApprovalHistoryAsync(history);

            return (true, "");
        }

        public async Task<List<ApprovalHistory>> GetApprovalHistoryForBroadcastAsync(int broadcastId)
        {
            return await _approvalRepository.GetHistoryForBroadcastAsync(broadcastId);
        }

    }
}
