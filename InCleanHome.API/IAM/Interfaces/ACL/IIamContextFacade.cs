namespace InCleanHome.API.IAM.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<int> FetchUserIdByEmail(string email);
    Task<string> FetchEmailByUserId(int userId);
    Task<string> FetchRoleByUserId(int userId);
}
