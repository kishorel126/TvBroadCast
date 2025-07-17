using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvBroadCast.Domain.Entities;

namespace TvBroadCast.Domain.Interfaces.IApproval
{
    public interface IApprovalRepository
    {

        Task<ApprovalHistory?> GetLatestForBroadcastAsync(int broadcastId);
        Task<List<ApprovalHistory>> GetHistoryForBroadcastAsync(int broadcastId);
        Task AddApprovalHistoryAsync(ApprovalHistory approvalHistory);


    }
}
