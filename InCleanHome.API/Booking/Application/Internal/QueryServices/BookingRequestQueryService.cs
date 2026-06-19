using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Domain.Model.Queries;
using InCleanHome.API.Booking.Domain.Repositories;
using InCleanHome.API.Booking.Domain.Services;

namespace InCleanHome.API.Booking.Application.Internal.QueryServices;


/// Servicio de aplicación interno encargado de procesar las consultas (operaciones de lectura) 
/// relacionadas con las solicitudes de reserva (BookingRequest).
/// Forma parte de la implementación del patrón CQRS, aislando la lógica de recuperación de datos.


public class BookingRequestQueryService(IBookingRequestRepository repository) : IBookingRequestQueryService
{
    public async Task<BookingRequest?> Handle(GetBookingByIdQuery query)
        => await repository.FindByIdAsync(query.Id);

    public async Task<IEnumerable<BookingRequest>> Handle(GetBookingsByClientUserIdQuery query)
        => await repository.FindByClientUserIdAsync(query.ClientUserId);

    public async Task<IEnumerable<BookingRequest>> Handle(GetBookingsByWorkerUserIdQuery query)
        => await repository.FindByWorkerUserIdAsync(query.WorkerUserId);
}
