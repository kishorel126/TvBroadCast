
namespace TvBroadCast.Domain.Entities
{
    public class ApprovalHistory
    {
        public int Id { get; set; }
        public int BroadcastId { get; set; }
        public string ApproverId { get; set; } = string.Empty;   // IdentityUser.Id
        public DateTime Timestamp { get; set; }
        public BroadcastStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;

    }

}