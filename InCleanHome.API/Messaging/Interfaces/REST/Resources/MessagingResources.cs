namespace InCleanHome.API.Messaging.Interfaces.REST.Resources;

/// <summary>
/// Recurso recibido desde el cliente para enviar un nuevo mensaje.
/// Representa el cuerpo de la petición (Payload / Request Body).
/// </summary>

public record SendMessageResource(string Content);

public record MessageResource(
    int Id,
    int SenderId,
    int RecipientId,
    string Content,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? ReadAt);

public record ConversationResource(
    int UserId,
    string UserName,
    string LastMessage,
    DateTimeOffset? LastMessageAt,
    int UnreadCount);
