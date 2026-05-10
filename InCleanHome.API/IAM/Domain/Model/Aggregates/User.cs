using System.Text.Json.Serialization;
using InCleanHome.API.IAM.Domain.Model.ValueObjects;

namespace InCleanHome.API.IAM.Domain.Model.Aggregates;

/// <summary>
///     User aggregate root for the IAM bounded context.
/// </summary>
public class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;

    [JsonIgnore] public string PasswordHash { get; private set; } = string.Empty;

    public string Role { get; private set; } = UserRole.Client;
    public bool IsVerified { get; private set; }
    public bool DocumentsVerified { get; private set; }

    public User() { }

    public User(string email, string passwordHash, string role)
    {
        Email          = email;
        PasswordHash   = passwordHash;
        Role           = UserRole.IsValid(role) ? role : UserRole.Client;
        // Clients are auto-verified on sign-up. Workers must upload documents and be approved.
        IsVerified         = role == UserRole.Client;
        DocumentsVerified  = role == UserRole.Client;
    }

    public User UpdatePasswordHash(string passwordHash) { PasswordHash = passwordHash; return this; }
    public User UpdateEmail(string email) { Email = email; return this; }
    public User Verify()                                { IsVerified = true; return this; }
    public User MarkDocumentsAsVerified()               { DocumentsVerified = true; IsVerified = true; return this; }
}
