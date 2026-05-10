namespace InCleanHome.API.Messaging.Interfaces.REST.Resources;

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
