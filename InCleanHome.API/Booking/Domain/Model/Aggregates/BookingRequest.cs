using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.CreatedUpdatedDate.Contracts;
using InCleanHome.API.Booking.Domain.Model.ValueObjects;

namespace InCleanHome.API.Booking.Domain.Model.Aggregates;

/// <summary>
///     Booking request aggregate root — a household hires a worker for a specific date/time.
/// </summary>
/// <remarks>
///     <c>ClientId</c> and <c>WorkerId</c> hold <em>user ids</em> (not profile ids), which is
///     how the frontend addresses everyone (messaging, navigation). The platform fee is fixed
///     at 10% (US-34/US-35) and is computed inside the aggregate so totals are consistent.
/// </remarks>
public class BookingRequest : IEntityWithCreatedUpdatedDate
{
    public const decimal PlatformFeeRate = 0.10m;

    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public int WorkerId { get; private set; }

    public string ServiceType { get; private set; } = string.Empty;
    public DateOnly Date { get; private set; }
    public string StartTime { get; private set; } = "00:00";
    public string EndTime { get; private set; }   = "00:00";
    public decimal Hours { get; private set; }

    public int PaymentMethodId { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public string Notes { get; private set; }   = string.Empty;

    public decimal HourlyRate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PlatformFee { get; private set; }
    public decimal WorkerEarning { get; private set; }

    public string Status { get; private set; } = BookingStatus.Pending;

    [Column("CreatedAt")] public DateTimeOffset? CreatedDate { get; set; }
    [Column("UpdatedAt")] public DateTimeOffset? UpdatedDate { get; set; }

    public BookingRequest() { }

    public BookingRequest(
        int clientId, int workerId, string serviceType, DateOnly date,
        string startTime, string endTime, decimal hours, int paymentMethodId,
        string address, string notes, decimal hourlyRate)
    {
        ClientId        = clientId;
        WorkerId        = workerId;
        ServiceType     = serviceType;
        Date            = date;
        StartTime       = startTime;
        EndTime         = endTime;
        Hours           = hours;
        PaymentMethodId = paymentMethodId;
        Address         = address ?? string.Empty;
        Notes           = notes ?? string.Empty;
        HourlyRate      = hourlyRate;
        TotalAmount     = Math.Round(hourlyRate * hours, 2);
        PlatformFee     = Math.Round(TotalAmount * PlatformFeeRate, 2);
        WorkerEarning   = TotalAmount - PlatformFee;
        Status          = BookingStatus.Pending;
    }

    // -------------- State transitions (encapsulated in the aggregate) --------------

    public BookingRequest Accept()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be accepted.");
        Status = BookingStatus.Accepted;
        return this;
    }

    public BookingRequest Reject()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be rejected.");
        Status = BookingStatus.Rejected;
        return this;
    }

    public BookingRequest CancelByClient()
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.Accepted)
            throw new InvalidOperationException("Only pending or accepted bookings can be cancelled.");
        Status = BookingStatus.Cancelled;
        return this;
    }

    public BookingRequest Complete()
    {
        if (Status != BookingStatus.Accepted)
            throw new InvalidOperationException("Only accepted bookings can be completed.");
        Status = BookingStatus.Completed;
        return this;
    }
}
