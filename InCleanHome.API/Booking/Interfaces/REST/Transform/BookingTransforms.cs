using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Interfaces.REST.Resources;

namespace InCleanHome.API.Booking.Interfaces.REST.Transform;

// Clase estática encargada de transformar las entidades de dominio en recursos DTO de salida para la API REST.
// Aplica el patrón Assembler para desacoplar el modelo de dominio de los contratos externos de la API.
public static class BookingResourceFromEntityAssembler
{
    public static BookingResource ToResourceFromEntity(BookingRequest b, string clientName, string workerName)
        => new(
            b.Id,                   // ID único de la reserva
            b.ClientId,             // ID del cliente propietario
            b.WorkerId,             // ID del trabajador asignado
            clientName,             // Nombre del cliente resuelto por el microservicio/módulo de perfiles
            workerName,             // Nombre del trabajador resuelto por el microservicio/módulo de perfiles
            b.ServiceType,          // Categoría del servicio brindado
            b.Date.ToString("yyyy-MM-dd"), // Formateo explícito de la fecha ISO para el consumo del frontend
            b.StartTime,            // Hora de inicio del servicio
            b.EndTime,              // Hora de finalización del servicio
            b.Hours,                // Total de horas calculadas
            b.PaymentMethodId,      // ID del método de pago
            b.Address,              // Ubicación del domicilio registrado
            b.Notes,                // Anotaciones o especificaciones de la reserva
            b.HourlyRate,           // Precio base por hora fijado
            b.TotalAmount,          // Monto total bruto calculado
            b.PlatformFee,          // Comisión deducida por la plataforma
            b.WorkerEarning,        // Ganancia neta correspondiente al trabajador
            b.Status,               // Representación en cadena del estado actual del ciclo de vida de la reserva
            b.CreatedDate);         // Sello de auditoría temporal de la creación de la reserva
}
