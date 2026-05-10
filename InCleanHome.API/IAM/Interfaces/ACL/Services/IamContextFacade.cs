using InCleanHome.API.IAM.Domain.Model.Queries;
using InCleanHome.API.IAM.Domain.Services;

namespace InCleanHome.API.IAM.Interfaces.ACL.Services;

public class IamContextFacade(IUserQueryService userQueryService) : IIamContextFacade
{
    public async Task<int> FetchUserIdByEmail(string email)
    {
        var user = await userQueryService.Handle(new GetUserByEmailQuery(email));
        return user?.Id ?? 0;
    }

    public async Task<string> FetchEmailByUserId(int userId)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId));
        return user?.Email ?? string.Empty;
    }

    public async Task<string> FetchRoleByUserId(int userId)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId));
        return user?.Role ?? string.Empty;
    }
}
