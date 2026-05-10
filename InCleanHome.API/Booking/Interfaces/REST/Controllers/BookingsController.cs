using System.Globalization;
using System.Net.Mime;
using InCleanHome.API.Booking.Domain.Model.Commands;
using InCleanHome.API.Booking.Domain.Model.Queries;
using InCleanHome.API.Booking.Domain.Model.ValueObjects;
using InCleanHome.API.Booking.Domain.Services;
using InCleanHome.API.Booking.Interfaces.REST.Resources;
using InCleanHome.API.Booking.Interfaces.REST.Transform;
using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.IAM.Domain.Model.ValueObjects;
using InCleanHome.API.Profiles.Interfaces.ACL;
using InCleanHome.API.Profiles.Domain.Model.Commands;
using InCleanHome.API.Profiles.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InCleanHome.API.Booking.Interfaces.REST.Controllers;

/// <summary>
///     Booking endpoints consumed by clients and workers.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item><description>POST /bookings — client creates a booking</description></item>
///         <item><description>GET /bookings — list current user's bookings (role-aware)</description></item>
///         <item><description>PATCH /bookings/{id}/status — change status</description></item>
///     </list>
/// </remarks>
[ApiController]
[Route("api/bookings")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Bookings — full hiring lifecycle")]
public class BookingsController(
    IBookingRequestCommandService commandService,
    IBookingRequestQueryService queryService,
    IProfilesContextFacade profilesFacade,
    IWorkerProfileCommandService workerCommandService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation("Create Booking", "A client creates a booking against a worker.")]
    public async Task<IActionResult> Create([FromBody] CreateBookingResource resource)
    {
        var current = (User?)HttpContext.Items["User"];
        if (current is null) return Unauthorized();
        if (current.Role != UserRole.Client) return Forbid();

        if (!DateOnly.TryParseExact(resource.Date, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return BadRequest(new { error = "Invalid date format (expected yyyy-MM-dd)" });

        try
        {
            var booking = await commandService.Handle(new CreateBookingCommand(
                current.Id, resource.WorkerId, resource.ServiceType, date,
                resource.StartTime, resource.EndTime, resource.Hours, resource.PaymentMethodId,
                resource.Address, resource.Notes));

            var clientName = await profilesFacade.FetchUserNameByUserId(booking.ClientId);
            var workerName = await profilesFacade.FetchUserNameByUserId(booking.WorkerId);
            return Ok(BookingResourceFromEntityAssembler.ToResourceFromEntity(booking, clientName, workerName));
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation("List My Bookings", "Returns bookings for the current user (client or worker view).")]
    public async Task<IActionResult> ListMine()
    {
        var current = (User?)HttpContext.Items["User"];
        if (current is null) return Unauthorized();

        var bookings = current.Role switch
        {
            UserRole.Worker => await queryService.Handle(new GetBookingsByWorkerUserIdQuery(current.Id)),
            UserRole.Client => await queryService.Handle(new GetBookingsByClientUserIdQuery(current.Id)),
            _               => Enumerable.Empty<Domain.Model.Aggregates.BookingRequest>()
        };

        var result = new List<BookingResource>();
        foreach (var b in bookings)
        {
            var clientName = await profilesFacade.FetchUserNameByUserId(b.ClientId);
            var workerName = await profilesFacade.FetchUserNameByUserId(b.WorkerId);
            result.Add(BookingResourceFromEntityAssembler.ToResourceFromEntity(b, clientName, workerName));
        }
        return Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    [SwaggerOperation("Update Booking Status",
        "Transitions a booking through its lifecycle (accepted/rejected/cancelled/completed).")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusResource resource)
    {
        var current = (User?)HttpContext.Items["User"];
        if (current is null) return Unauthorized();

        if (!BookingStatus.IsValid(resource.Status))
            return BadRequest(new { error = "Invalid status" });

        try
        {
            var booking = await commandService.Handle(
                new UpdateBookingStatusCommand(id, current.Id, current.Role, resource.Status));
            if (booking is null) return NotFound();

            var clientName = await profilesFacade.FetchUserNameByUserId(booking.ClientId);
            var workerName = await profilesFacade.FetchUserNameByUserId(booking.WorkerId);
            return Ok(BookingResourceFromEntityAssembler.ToResourceFromEntity(booking, clientName, workerName));
        }
        catch (UnauthorizedAccessException e) { return StatusCode(403, new { error = e.Message }); }
        catch (Exception e)                   { return BadRequest(new { error = e.Message }); }
    }
}
