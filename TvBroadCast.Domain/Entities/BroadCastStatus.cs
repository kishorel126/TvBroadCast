

namespace TvBroadCast.Domain.Entities
{
    public class BroadCastStatus { 
        public enum BStatus
        {
            Pending,     // Awaiting approval
            Approved,    // Approved and visible to all
            Rejected     // Rejected, sent back with comments
        }
    }
}
