namespace InCleanHome.API.Profiles.Interfaces.ACL;

/// <summary>
///     ACL facade exposing Profiles operations to other bounded contexts (read-only essentials).
/// </summary>

// Anti-Corruption Layer (ACL) Facade.
// Acts as a unified and secure entry point for other modules/contexts 
// to interact with the 'Profiles' module without tight internal coupling.

public interface IProfilesContextFacade
{
    Task<string> FetchUserNameByUserId(int userId);
    Task<decimal> FetchWorkerHourlyRateByUserId(int userId);
    Task RegisterWorkerCompletedService(int workerUserId, int rating);
}
