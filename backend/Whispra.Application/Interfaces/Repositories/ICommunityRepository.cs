using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface ICommunityRepository
{
    Task<Community?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Community> CreateAsync(Community community, CancellationToken cancellationToken = default);
    Task UpdateAsync(Community community, CancellationToken cancellationToken = default);
    Task<List<Community>> GetPublicCommunitiesAsync(int skip, int limit, CancellationToken cancellationToken = default);
    Task<List<Community>> GetUserCommunitiesAsync(string userId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}