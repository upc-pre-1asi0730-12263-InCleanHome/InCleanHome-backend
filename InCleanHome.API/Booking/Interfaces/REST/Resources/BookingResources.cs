namespace InCleanHome.API.Booking.Interfaces.REST.Resources;

// Recurso que representa los datos necesarios enviados por el frontend para solicitar una nueva reserva.
public record CreateBookingResource(
    int WorkerId,
    string ServiceType,
    string Date,           // "yyyy-MM-dd" from the frontend calendar
    string StartTime,      // "HH:mm"
    string EndTime,        // "HH:mm"
    decimal Hours,
    int PaymentMethodId,
    string Address,
    string? Notes);

// Recurso que contiene la carga útil mínima para solicitar la actualización del estado de una reserva.
public record UpdateBookingStatusResource(string Status);

// Recurso de salida que representa la respuesta estructurada con todo el detalle de la reserva para el cliente o trabajador.
public record BookingResource(
    int Id,                // Identificador único de la reserva en el sistema
    int ClientId,          // ID del cliente dueño de la solicitud
    int WorkerId,          // ID del profesional de limpieza asignado
    string ClientName,     // Nombre completo del cliente (resuelto mediante ACL/Profiles)
    string WorkerName,     // Nombre completo del trabajador (resuelto mediante ACL/Profiles)
    string ServiceType,    // Tipo de servicio de limpieza pactado
    string Date,           // Fecha programada del servicio en formato de texto para el cliente
    string StartTime,      // Hora de inicio pactada
    string EndTime,        // Hora de finalización pactada
    decimal Hours,         // Duración total del servicio en horas
    int PaymentMethodId,   // ID del método de pago utilizado
    string Address,        // Dirección donde se ejecuta o ejecutó el servicio
    string Notes,          // Notas o comentarios asociados a la reserva
    decimal HourlyRate,    // Tarifa por hora cobrada por el trabajador al momento de la reserva
    decimal TotalAmount,   // Costo total bruto calculado del servicio (Horas * Tarifa)
    decimal PlatformFee,   // Comisión retenida por la plataforma de intermediación
    decimal WorkerEarning, // Ganancia neta final asignada al monedero del trabajador
    string Status,         // Estado actual de la reserva en su ciclo de vida
    DateTimeOffset? CreatedAt // Marca de tiempo exacta en que se registró la solicitud
);
