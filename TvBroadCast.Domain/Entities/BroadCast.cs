

namespace TvBroadCast.Domain.Entities
{
    public class BroadCast
    {

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BroadcastStatus Status { get; set; } = BroadcastStatus.Pending;
        public string? ApproverComment { get; set; }
        public string SchedulerId { get; set; } = string.Empty;  // IdentityUser.Id
        public string? ApproverId { get; set; } // IdentityUser.Id

    }
}
