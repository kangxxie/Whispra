namespace Whispra.Domain.Entities.Communities;

public class CommunityInvite : BaseEntity
{
    public string CommunityId { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int? MaxUses { get; set; } // null = unlimited
    public int UsesCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}