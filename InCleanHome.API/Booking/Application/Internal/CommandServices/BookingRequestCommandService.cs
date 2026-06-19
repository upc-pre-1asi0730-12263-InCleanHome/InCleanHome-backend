using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Domain.Model.Commands;
using InCleanHome.API.Booking.Domain.Model.ValueObjects;
using InCleanHome.API.Booking.Domain.Repositories;
using InCleanHome.API.Booking.Domain.Services;
using InCleanHome.API.IAM.Domain.Model.ValueObjects;
using InCleanHome.API.Profiles.Domain.Model.Queries;
using InCleanHome.API.Profiles.Domain.Services;
using InCleanHome.API.Profiles.Interfaces.ACL;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.API.Booking.Application.Internal.CommandServices;

public class BookingRequestCommandService(
    IBookingRequestRepository repository,
    IWorkerProfileQueryService workerQueryService,
    IProfilesContextFacade profilesFacade,
    IUnitOfWork unitOfWork) : IBookingRequestCommandService
{
    public async Task<BookingRequest> Handle(CreateBookingCommand c)
    {
        var worker = await workerQueryService.Handle(new GetWorkerProfileByUserIdQuery(c.WorkerId))
            ?? throw new Exception("Worker not found");

        var booking = new BookingRequest(
            c.ClientId, c.WorkerId, c.ServiceType, c.Date,
            c.StartTime, c.EndTime, c.Hours, c.PaymentMethodId,
            c.Address, c.Notes ?? string.Empty, worker.HourlyRate);

        await repository.AddAsync(booking);
        await unitOfWork.CompleteAsync();
        return booking;
    }

    public async Task<BookingRequest?> Handle(UpdateBookingStatusCommand c)
    {
        var booking = await repository.FindByIdAsync(c.BookingId);
        if (booking is null) return null;

        // Authorization rules per role
        var isClient = c.RequesterRole == UserRole.Client && booking.ClientId == c.RequesterUserId;
        var isWorker = c.RequesterRole == UserRole.Worker && booking.WorkerId == c.RequesterUserId;
        var isAdmin  = c.RequesterRole == UserRole.Admin;
        if (!isClient && !isWorker && !isAdmin)
            throw new UnauthorizedAccessException("Not allowed to change this booking");

        switch (c.NewStatus)
        {
            case BookingStatus.Accepted:
                if (!isWorker && !isAdmin) throw new UnauthorizedAccessException("Only workers can accept bookings");
                booking.Accept();
                break;
            case BookingStatus.Rejected:
                if (!isWorker && !isAdmin) throw new UnauthorizedAccessException("Only workers can reject bookings");
                booking.Reject();
                break;
            case BookingStatus.Cancelled:
                if (!isClient && !isAdmin) throw new UnauthorizedAccessException("Only clients can cancel bookings");
                booking.CancelByClient();
                break;
            case BookingStatus.Completed:
                if (!isWorker && !isAdmin) throw new UnauthorizedAccessException("Only workers can complete bookings");
                booking.Complete();
                break;
            default:
                throw new InvalidOperationException($"Unsupported status transition '{c.NewStatus}'");
        }

        repository.Update(booking);
        await unitOfWork.CompleteAsync();
        return booking;
    }
}
