using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Domain.Repositories;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InCleanHome.API.Booking.Infrastructure.Persistence.EFC.Repositories;

// Implementación del repositorio de infraestructura utilizando Entity Framework Core.
// Hereda la funcionalidad de BaseRepository y cumple con el contrato definido en el dominio.
public class BookingRequestRepository(AppDbContext context)
    : BaseRepository<BookingRequest>(context), IBookingRequestRepository
{
    // Recupera las reservas de un cliente específico, ordenándolas desde la más reciente.
    public async Task<IEnumerable<BookingRequest>> FindByClientUserIdAsync(int clientUserId)
        => await Context.Set<BookingRequest>()
            .Where(b => b.ClientId == clientUserId)
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();

    // Recupera las reservas asignadas a un trabajador específico, ordenándolas desde la más reciente.
    public async Task<IEnumerable<BookingRequest>> FindByWorkerUserIdAsync(int workerUserId)
        => await Context.Set<BookingRequest>()
            .Where(b => b.WorkerId == workerUserId)
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();
}
