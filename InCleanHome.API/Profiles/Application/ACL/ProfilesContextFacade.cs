using InCleanHome.API.Profiles.Domain.Model.Commands;
using InCleanHome.API.Profiles.Domain.Model.Queries;
using InCleanHome.API.Profiles.Domain.Services;

namespace InCleanHome.API.Profiles.Application.ACL;

public class ProfilesContextFacade(
    IClientProfileQueryService clientQueryService,
    IWorkerProfileQueryService workerQueryService,
    IWorkerProfileCommandService workerCommandService) : Profiles.Interfaces.ACL.IProfilesContextFacade
{
    public async Task<string> FetchUserNameByUserId(int userId)
    {
        var worker = await workerQueryService.Handle(new GetWorkerProfileByUserIdQuery(userId));
        if (worker != null) return worker.Name;
        var client = await clientQueryService.Handle(new GetClientProfileByUserIdQuery(userId));
        return client?.Name ?? string.Empty;
    }

    public async Task<decimal> FetchWorkerHourlyRateByUserId(int userId)
    {
        var worker = await workerQueryService.Handle(new GetWorkerProfileByUserIdQuery(userId));
        return worker?.HourlyRate ?? 0m;
    }

    public async Task RegisterWorkerCompletedService(int workerUserId, int rating)
        => await workerCommandService.Handle(new RegisterWorkerCompletedServiceCommand(workerUserId, rating));
}
