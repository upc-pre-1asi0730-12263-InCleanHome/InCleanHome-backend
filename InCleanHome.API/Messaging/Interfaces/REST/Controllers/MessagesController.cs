using System.Net.Mime;
using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.Messaging.Domain.Model.Commands;
using InCleanHome.API.Messaging.Domain.Model.Queries;
using InCleanHome.API.Messaging.Domain.Services;
using InCleanHome.API.Messaging.Interfaces.REST.Resources;
using InCleanHome.API.Messaging.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InCleanHome.API.Messaging.Interfaces.REST.Controllers;

/// <summary>
///     Direct messaging endpoints between users.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item><description>GET /messages/conversations</description></item>
///         <item><description>GET /messages/{userId}</description></item>
///         <item><description>POST /messages/{userId}</description></item>
///     </list>
/// </remarks>
[ApiController]
[Route("api/messages")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Direct messaging")]
public class MessagesController(
    IMessageCommandService commandService,
    IMessageQueryService queryService) : ControllerBase
{
    [HttpGet("conversations")]
    [SwaggerOperation("List Conversations", "Returns one row per peer with last message and unread count.")]
    public async Task<IActionResult> ListConversations()
    {
        var current = (User?)HttpContext.Items["User"];
        if (current is null) return Unauthorized();

        var convs = await queryService.Handle(new GetConversationsForUserQuery(current.Id));
        return Ok(convs.Select(ConversationResourceFromViewAssembler.ToResourceFromView));
    }

    [HttpGet("{userId:int}")]
    [SwaggerOperation("Get Thread", "Returns the full thread between the current user and the given userId.")]
    public async Task<IActionResult> GetThread(int userId)
    {
        var current = (User?)HttpContext.Items["User"];
        if (current is null) return Unauthorized();

        var messages = await queryService.Handle(new GetMessagesBetweenQuery(current.Id, userId));
        // Mark as read while we're at it (RFU: open chat == read)
        await commandService.Handle(new MarkConversationAsReadCommand(current.Id, userId));

        return Ok(messages.Select(MessageResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost("{userId:int}")]
    [SwaggerOperation("Send Message")]
    public async Task<IActionResult> Send(int userId, [FromBody] SendMessageResource resource)
    {
        var current = (User?)HttpContext.Items["User"];
        if (current is null) return Unauthorized();

        try
        {
            var m = await commandService.Handle(new SendMessageCommand(current.Id, userId, resource.Content));
            return Ok(MessageResourceFromEntityAssembler.ToResourceFromEntity(m));
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }
}
