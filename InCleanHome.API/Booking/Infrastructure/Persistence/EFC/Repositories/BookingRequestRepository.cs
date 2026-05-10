using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Domain.Repositories;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InCleanHome.API.Booking.Infrastructure.Persistence.EFC.Repositories;

public class BookingRequestRepository(AppDbContext context)
    : BaseRepository<BookingRequest>(context), IBookingRequestRepository
{
    public async Task<IEnumerable<BookingRequest>> FindByClientUserIdAsync(int clientUserId)
        => await Context.Set<BookingRequest>()
            .Where(b => b.ClientId == clientUserId)
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();

    public async Task<IEnumerable<BookingRequest>> FindByWorkerUserIdAsync(int workerUserId)
        => await Context.Set<BookingRequest>()
            .Where(b => b.WorkerId == workerUserId)
            .OrderByDescending(b => b.CreatedDate)
            .ToListAsync();
}
