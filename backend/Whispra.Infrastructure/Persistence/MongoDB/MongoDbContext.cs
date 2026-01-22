using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
    public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    public IMongoCollection<CommunityMember> CommunityMembers => _database.GetCollection<CommunityMember>("community_members");
    public IMongoCollection<CommunityInvite> CommunityInvites => _database.GetCollection<CommunityInvite>("community_invites");
}