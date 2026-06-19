using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Domain.Model.Commands;
using InCleanHome.API.Booking.Domain.Model.Queries;

namespace InCleanHome.API.Booking.Domain.Services;

// Interfaz para el servicio de aplicación encargado de procesar los comandos de reservas (escritura).
public interface IBookingRequestCommandService
{
    Task<BookingRequest> Handle(CreateBookingCommand command);
    Task<BookingRequest?> Handle(UpdateBookingStatusCommand command);
}

// Interfaz para el servicio de aplicación encargado de procesar las consultas de reservas (lectura).
public interface IBookingRequestQueryService
{
    Task<BookingRequest?> Handle(GetBookingByIdQuery query);
    Task<IEnumerable<BookingRequest>> Handle(GetBookingsByClientUserIdQuery query);
    Task<IEnumerable<BookingRequest>> Handle(GetBookingsByWorkerUserIdQuery query);
}
