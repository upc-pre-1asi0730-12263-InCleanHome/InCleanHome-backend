using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Interfaces.REST.Resources;

namespace InCleanHome.API.Booking.Interfaces.REST.Transform;

public static class BookingResourceFromEntityAssembler
{
    public static BookingResource ToResourceFromEntity(BookingRequest b, string clientName, string workerName)
        => new(
            b.Id,
            b.ClientId,
            b.WorkerId,
            clientName,
            workerName,
            b.ServiceType,
            b.Date.ToString("yyyy-MM-dd"),
            b.StartTime,
            b.EndTime,
            b.Hours,
            b.PaymentMethodId,
            b.Address,
            b.Notes,
            b.HourlyRate,
            b.TotalAmount,
            b.PlatformFee,
            b.WorkerEarning,
            b.Status,
            b.CreatedDate);
}


using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Interfaces.REST.Resources;

namespace InCleanHome.API.Booking.Interfaces.REST.Transform;

public static class BookingResourceFromEntityAssembler
{
    public static BookingResource ToResourceFromEntity(BookingRequest b, string clientName, string workerName)
        => new(
            b.Id,
            b.ClientId,
            b.WorkerId,
            clientName,
            workerName,
            b.ServiceType,
            b.Date.ToString("yyyy-MM-dd"),
            b.StartTime,
            b.EndTime,
            b.Hours,
            b.PaymentMethodId,
            b.Address,
            b.Notes,
            b.HourlyRate,
            b.TotalAmount,
            b.PlatformFee,
            b.WorkerEarning,
            b.Status,
            b.CreatedDate);
}
