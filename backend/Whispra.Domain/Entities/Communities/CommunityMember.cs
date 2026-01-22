using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Communities;

public class CommunityMember : BaseEntity
{
    public string CommunityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public CommunityRole Role { get; set; } = CommunityRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}