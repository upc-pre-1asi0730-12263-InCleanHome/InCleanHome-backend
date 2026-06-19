using InCleanHome.API.Profiles.Domain.Model.Aggregates;
using InCleanHome.API.Profiles.Domain.Repositories;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InCleanHome.API.Profiles.Infrastructure.Persistence.EFC.Repositories;

// Infrastructure Repository Implementation using Entity Framework Core.
// Translates domain repository operations into actual database queries.
// Directly interacts with the DbContext.

public class ClientProfileRepository(AppDbContext context)
    : BaseRepository<ClientProfile>(context), IClientProfileRepository
{
    public async Task<ClientProfile?> FindByUserIdAsync(int userId)
        => await Context.Set<ClientProfile>().FirstOrDefaultAsync(c => c.UserId == userId);
}
