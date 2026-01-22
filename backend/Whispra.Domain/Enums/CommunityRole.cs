namespace Whispra.Domain.Enums;

public enum CommunityRole
{
    Member = 0,      // Can view posts, comment, create posts
    Moderator = 1,   // + Can moderate content, invite members
    Owner = 2        // + Can change settings, manage roles, delete community
}