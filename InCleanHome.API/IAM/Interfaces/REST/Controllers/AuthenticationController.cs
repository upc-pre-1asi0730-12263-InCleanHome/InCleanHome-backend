using System.Net.Mime;
using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.IAM.Domain.Model.Commands;
using InCleanHome.API.IAM.Domain.Model.Queries;
using InCleanHome.API.IAM.Domain.Model.ValueObjects;
using InCleanHome.API.IAM.Domain.Services;
using InCleanHome.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using InCleanHome.API.Profiles.Domain.Model.Commands;
using InCleanHome.API.Profiles.Domain.Model.Queries;
using InCleanHome.API.Profiles.Domain.Services;
using InCleanHome.API.Profiles.Interfaces.REST.Resources;
using InCleanHome.API.Profiles.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InCleanHome.API.IAM.Interfaces.REST.Controllers;

/// <summary>
///     Authentication endpoints consumed by the Vue frontend.
/// </summary>
/// <remarks>
///     Frontend wiring (see <c>src/IAM/views/*.vue</c>):
///     <list type="bullet">
///         <item><description>POST /auth/login</description></item>
///         <item><description>POST /auth/register/client</description></item>
///         <item><description>POST /auth/register/worker</description></item>
///         <item><description>POST /auth/worker/upload-document</description></item>
///     </list>
/// </remarks>
[ApiController]
[Route("api/auth")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Authentication & worker onboarding")]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService,
    IClientProfileCommandService clientProfileCommandService,
    IClientProfileQueryService clientProfileQueryService,
    IWorkerProfileCommandService workerProfileCommandService,
    IWorkerProfileQueryService workerProfileQueryService) : ControllerBase
{
    public record LoginResource(string Email, string Password);

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation("Sign In", "Authenticate a user; returns user payload + JWT.")]
    public async Task<IActionResult> Login([FromBody] LoginResource resource)
    {
        try
        {
            var (user, token) = await userCommandService.Handle(new SignInCommand(resource.Email, resource.Password));
            var (name, phone) = await ResolveNamePhone(user);
            var payload = UserPayloadFromEntityAssembler.FromUserAndProfile(user, name, phone);
            return Ok(new AuthResponseResource(payload, token));
        }
        catch (Exception e) when (e.Message.Contains("Documents not verified", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(403, new { error = e.Message });
        }
        catch (Exception e)
        {
            return Unauthorized(new { error = e.Message });
        }
    }

    [HttpPost("register/client")]
    [AllowAnonymous]
    [SwaggerOperation("Register Client", "Create a client account + profile.")]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterClientResource resource)
    {
        try
        {
            var user = await userCommandService.Handle(new SignUpCommand(resource.Email, resource.Password, UserRole.Client));
            await clientProfileCommandService.Handle(
                new CreateClientProfileCommand(user.Id, resource.Name, resource.Phone ?? string.Empty));

            var token = userCommandService.GenerateTokenFor(user);
            var payload = UserPayloadFromEntityAssembler.FromUserAndProfile(user, resource.Name, resource.Phone);
            return Ok(new AuthResponseResource(payload, token));
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("register/worker")]
    [AllowAnonymous]
    [SwaggerOperation("Register Worker", "Create a worker account + profile (pending document verification).")]
    public async Task<IActionResult> RegisterWorker([FromBody] RegisterWorkerResource resource)
    {
        if (!Profiles.Domain.Model.ValueObjects.Gender.IsValid(resource.Gender))
            return BadRequest(new { error = "Invalid gender" });

        if (resource.ServiceTypes == null || resource.ServiceTypes.Count == 0)
            return BadRequest(new { error = "Select at least one service type" });

        try
        {
            var user = await userCommandService.Handle(new SignUpCommand(resource.Email, resource.Password, UserRole.Worker));
            await workerProfileCommandService.Handle(new CreateWorkerProfileCommand(
                user.Id, resource.Name, resource.Phone ?? string.Empty,
                resource.Age, resource.Gender,
                resource.ServiceTypes, resource.Zones ?? new(),
                resource.HourlyRate, resource.ExperienceYears, resource.Bio ?? string.Empty));

            var token = userCommandService.GenerateTokenFor(user);
            var payload = UserPayloadFromEntityAssembler.FromUserAndProfile(user, resource.Name, resource.Phone);
            return Ok(new AuthResponseResource(payload, token));
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    public record UploadDocumentResource(string DocumentType, string FileBase64, string FileName);

    [HttpPost("worker/upload-document")]
    [SwaggerOperation("Upload Worker Document", "Upload a PDF (background_check or experience).")]
    public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentResource resource)
    {
        var current = (User?)HttpContext.Items["User"];
        if (current is null) return Unauthorized();
        if (current.Role != UserRole.Worker) return Forbid();

        try
        {
            await userCommandService.Handle(new UploadWorkerDocumentCommand(
                current.Id, resource.DocumentType, resource.FileName, resource.FileBase64));
            return Ok(new { message = "Document uploaded successfully" });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------
    private async Task<(string name, string? phone)> ResolveNamePhone(User user)
    {
        if (user.Role == UserRole.Worker)
        {
            var w = await workerProfileQueryService.Handle(new GetWorkerProfileByUserIdQuery(user.Id));
            return (w?.Name ?? user.Email, w?.Phone);
        }
        else
        {
            var c = await clientProfileQueryService.Handle(new GetClientProfileByUserIdQuery(user.Id));
            return (c?.Name ?? user.Email, c?.Phone);
        }
    }
}
