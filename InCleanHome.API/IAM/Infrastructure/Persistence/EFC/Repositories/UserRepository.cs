using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.IAM.Domain.Repositories;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InCleanHome.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class UserRepository(AppDbContext context)
    : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> FindByEmailAsync(string email)
        => await Context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);

    public bool ExistsByEmail(string email)
        => Context.Set<User>().Any(u => u.Email == email);
}
