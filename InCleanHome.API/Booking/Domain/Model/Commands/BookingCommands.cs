namespace InCleanHome.API.Booking.Domain.Model.Commands;

public record CreateBookingCommand(
    int ClientId,
    int WorkerId,
    string ServiceType,
    DateOnly Date,
    string StartTime,
    string EndTime,
    decimal Hours,
    int PaymentMethodId,
    string Address,
    string? Notes);

public record UpdateBookingStatusCommand(int BookingId, int RequesterUserId, string RequesterRole, string NewStatus);
