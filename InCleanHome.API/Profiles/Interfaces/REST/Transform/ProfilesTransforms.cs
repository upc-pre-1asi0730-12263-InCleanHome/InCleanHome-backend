using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.Profiles.Domain.Model.Aggregates;
using InCleanHome.API.Profiles.Interfaces.REST.Resources;

namespace InCleanHome.API.Profiles.Interfaces.REST.Transform;

// Assembler / Mapper.
// Contains utility functions to safely transform API Resources/DTOs 
// into Domain Commands/Entities, and vice versa.

public static class WorkerResourceFromEntityAssembler
{
    /// <summary>
    ///     The frontend identifies a worker by user id (used for /worker/{id} links and
    ///     messaging routes), not by WorkerProfile.Id, so we expose <c>UserId</c> as <c>id</c>.
    /// </summary>
    public static WorkerResource ToResourceFromEntity(WorkerProfile w, bool documentsVerified)
        => new(
            w.UserId,
            w.Name,
            w.Phone,
            w.Age,
            w.Gender,
            w.ServiceTypes,
            w.Zones,
            w.HourlyRate,
            w.ExperienceYears,
            w.Bio,
            w.AverageRating,
            w.TotalServices,
            documentsVerified);
}

public static class UserPayloadFromEntityAssembler
{
    public static UserPayload FromUserAndProfile(User user, string name, string? phone)
        => new(user.Id, user.Email, user.Role, name, phone, user.IsVerified, user.DocumentsVerified);
}

public static class ClientResourceFromEntityAssembler
{
    public static ClientProfileResource ToResourceFromEntity(ClientProfile c)
        => new(
            c.Id,
            c.UserId,
            c.Name,
            c.Phone);
}
