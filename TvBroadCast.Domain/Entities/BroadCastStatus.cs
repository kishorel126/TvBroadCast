

namespace TvBroadCast.Domain.Entities
{
        public enum BroadcastStatus
        {
            Pending,     // Awaiting approval
            Approved,    // Approved and visible to all
            Rejected     // Rejected, sent back with comments
        }
}
