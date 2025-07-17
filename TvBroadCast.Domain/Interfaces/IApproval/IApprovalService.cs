using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvBroadCast.Domain.Entities;

namespace TvBroadCast.Domain.Interfaces.IApproval
{
    public interface IApprovalService
    {

        Task<(bool Success, string Error)> ApproveBroadCastAsync(int broadcastId , string approverId , string comment);

        Task<(bool Success, string Error)> RejectBroadCastAsync(int broadcastId, string approverId, string comment);

        Task<List<ApprovalHistory>> GetApprovalHistoryForBroadcastAsync(int broadcastId);

    }
}
