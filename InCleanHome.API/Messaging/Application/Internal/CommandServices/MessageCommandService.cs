using InCleanHome.API.Messaging.Domain.Model.Aggregates;
using InCleanHome.API.Messaging.Domain.Model.Commands;
using InCleanHome.API.Messaging.Domain.Repositories;
using InCleanHome.API.Messaging.Domain.Services;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.API.Messaging.Application.Internal.CommandServices;

public class MessageCommandService(
    IMessageRepository repository,
    IUnitOfWork unitOfWork) : IMessageCommandService
{
    public async Task<Message> Handle(SendMessageCommand c)
    {
        if (c.SenderId == c.RecipientId)
            throw new Exception("Cannot send a message to yourself.");
        if (string.IsNullOrWhiteSpace(c.Content))
            throw new Exception("Message content cannot be empty.");

        var message = new Message(c.SenderId, c.RecipientId, c.Content);
        await repository.AddAsync(message);
        await unitOfWork.CompleteAsync();
        return message;
    }

    public async Task Handle(MarkConversationAsReadCommand c)
    {
        await repository.MarkAsReadAsync(c.UserId, c.OtherUserId);
        await unitOfWork.CompleteAsync();
    }
}
