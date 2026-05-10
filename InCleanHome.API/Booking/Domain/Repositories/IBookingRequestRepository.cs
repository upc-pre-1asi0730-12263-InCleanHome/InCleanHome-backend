using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.API.Booking.Domain.Repositories;

public interface IBookingRequestRepository : IBaseRepository<BookingRequest>
{
    Task<IEnumerable<BookingRequest>> FindByClientUserIdAsync(int clientUserId);
    Task<IEnumerable<BookingRequest>> FindByWorkerUserIdAsync(int workerUserId);
}
