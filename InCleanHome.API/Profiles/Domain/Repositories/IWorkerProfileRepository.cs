using InCleanHome.API.Profiles.Domain.Model.Aggregates;
using InCleanHome.API.Profiles.Domain.Model.Queries;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.API.Profiles.Domain.Repositories;

// Repository Interface.
// Defines the contract (read/write methods) required by the domain to persist data.
// The concrete implementation is deferred to the Infrastructure layer.

public interface IWorkerProfileRepository : IBaseRepository<WorkerProfile>
{
    Task<WorkerProfile?> FindByUserIdAsync(int userId);
    Task<IEnumerable<WorkerProfile>> SearchAsync(SearchWorkersQuery filters);
}
