using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Communities;

public class Community : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public CommunityPrivacy Privacy { get; set; } = CommunityPrivacy.Public;
    public string OwnerId { get; set; } = string.Empty;
    public int MemberCount { get; set; } = 0;
    public List<string> Tags { get; set; } = new();
}