using InCleanHome.API.Profiles.Domain.Model.Aggregates;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.API.Profiles.Domain.Repositories;

// Repository Interface.
// Defines the contract (read/write methods) required by the domain to persist data.
// The concrete implementation is deferred to the Infrastructure layer.

public interface IClientProfileRepository : IBaseRepository<ClientProfile>
{
    Task<ClientProfile?> FindByUserIdAsync(int userId);
}
