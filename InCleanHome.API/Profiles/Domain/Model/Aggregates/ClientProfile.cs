using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.CreatedUpdatedDate.Contracts;

namespace InCleanHome.API.Profiles.Domain.Model.Aggregates;

/// <summary>
///     Client profile aggregate root — household demanding domestic services.
/// </summary>
public class ClientProfile : IEntityWithCreatedUpdatedDate
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;

    [Column("CreatedAt")] public DateTimeOffset? CreatedDate { get; set; }
    [Column("UpdatedAt")] public DateTimeOffset? UpdatedDate { get; set; }

    public ClientProfile() { }

    public ClientProfile(int userId, string name, string phone)
    {
        UserId = userId;
        Name   = name;
        Phone  = phone ?? string.Empty;
    }

    public ClientProfile Update(string name, string phone)
    {
        Name  = name;
        Phone = phone ?? string.Empty;
        return this;
    }
}
