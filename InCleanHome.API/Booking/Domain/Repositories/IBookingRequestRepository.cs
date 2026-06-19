using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.API.Booking.Domain.Repositories;

// Interfaz del repositorio para la gestión de persistencia del agregado BookingRequest.
// Hereda las operaciones CRUD básicas del repositorio base.

public interface IBookingRequestRepository : IBaseRepository<BookingRequest>
{
    Task<IEnumerable<BookingRequest>> FindByClientUserIdAsync(int clientUserId);
    Task<IEnumerable<BookingRequest>> FindByWorkerUserIdAsync(int workerUserId);
}
