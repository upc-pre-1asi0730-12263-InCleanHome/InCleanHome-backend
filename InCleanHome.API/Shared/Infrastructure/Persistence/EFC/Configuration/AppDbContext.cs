using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.Messaging.Domain.Model.Aggregates;
using InCleanHome.API.Payments.Domain.Model.Aggregates;
using InCleanHome.API.Profiles.Domain.Model.Aggregates;
using InCleanHome.API.ReviewsAndEvaluation.Domain.Model.Aggregates;
using InCleanHome.API.SearchAndCatalog.Domain.Model.Aggregates;
using InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace InCleanHome.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context for the InCleanHome platform.
/// </summary>
/// <remarks>
///     Aggregates the persistence configuration of every bounded context: IAM,
///     Profiles, SearchAndCatalog, Booking, Payments, ReviewsAndEvaluation, Messaging.
///     <para>
///     Snake-case naming convention is applied at the end of <c>OnModelCreating</c>, so
///     C# <c>WorkerProfile.HourlyRate</c> ends up as <c>worker_profiles.hourly_rate</c> in
///     PostgreSQL.
///     </para>
/// </remarks>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ====================================================================
        // IAM
        // ====================================================================
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(120);
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
        builder.Entity<User>().Property(u => u.Role).IsRequired().HasMaxLength(20);
        builder.Entity<User>().Property(u => u.IsVerified).HasDefaultValue(false);
        builder.Entity<User>().Property(u => u.DocumentsVerified).HasDefaultValue(false);
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        builder.Entity<WorkerDocument>().HasKey(d => d.Id);
        builder.Entity<WorkerDocument>().Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WorkerDocument>().Property(d => d.UserId).IsRequired();
        builder.Entity<WorkerDocument>().Property(d => d.DocumentType).IsRequired().HasMaxLength(40);
        builder.Entity<WorkerDocument>().Property(d => d.FileName).IsRequired().HasMaxLength(200);
        builder.Entity<WorkerDocument>().Property(d => d.FileBase64).IsRequired();

        // ====================================================================
        // Profiles
        // ====================================================================
        builder.Entity<ClientProfile>().HasKey(c => c.Id);
        builder.Entity<ClientProfile>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ClientProfile>().Property(c => c.UserId).IsRequired();
        builder.Entity<ClientProfile>().Property(c => c.Name).IsRequired().HasMaxLength(120);
        builder.Entity<ClientProfile>().Property(c => c.Phone).HasMaxLength(20);
        builder.Entity<ClientProfile>().HasIndex(c => c.UserId).IsUnique();

        builder.Entity<WorkerProfile>().HasKey(w => w.Id);
        builder.Entity<WorkerProfile>().Property(w => w.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WorkerProfile>().Property(w => w.UserId).IsRequired();
        builder.Entity<WorkerProfile>().Property(w => w.Name).IsRequired().HasMaxLength(120);
        builder.Entity<WorkerProfile>().Property(w => w.Phone).HasMaxLength(20);
        builder.Entity<WorkerProfile>().Property(w => w.Age);
        builder.Entity<WorkerProfile>().Property(w => w.Gender).HasMaxLength(20);
        builder.Entity<WorkerProfile>().Property(w => w.HourlyRate).HasPrecision(10, 2);
        builder.Entity<WorkerProfile>().Property(w => w.ExperienceYears);
        builder.Entity<WorkerProfile>().Property(w => w.Bio).HasMaxLength(1000);
        builder.Entity<WorkerProfile>().Property(w => w.AverageRating).HasPrecision(3, 2);
        builder.Entity<WorkerProfile>().Property(w => w.TotalServices);
        builder.Entity<WorkerProfile>().HasIndex(w => w.UserId).IsUnique();

        // PostgreSQL text[] mapping for the multi-value lists. Npgsql's provider maps
        // List<string> to text[] natively when the column type is set explicitly.
        builder.Entity<WorkerProfile>().Property(w => w.ServiceTypes).HasColumnType("text[]");
        builder.Entity<WorkerProfile>().Property(w => w.Zones).HasColumnType("text[]");

        // ====================================================================
        // SearchAndCatalog
        // ====================================================================
        builder.Entity<AvailabilitySlot>().HasKey(a => a.Id);
        builder.Entity<AvailabilitySlot>().Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AvailabilitySlot>().Property(a => a.WorkerUserId).IsRequired();
        builder.Entity<AvailabilitySlot>().Property(a => a.DayOfWeek).IsRequired();
        builder.Entity<AvailabilitySlot>().Property(a => a.StartTime).IsRequired().HasMaxLength(5);
        builder.Entity<AvailabilitySlot>().Property(a => a.EndTime).IsRequired().HasMaxLength(5);
        builder.Entity<AvailabilitySlot>().Property(a => a.IsAvailable).HasDefaultValue(true);
        builder.Entity<AvailabilitySlot>().HasIndex(a => new { a.WorkerUserId, a.DayOfWeek });

        // ====================================================================
        // Booking
        // ====================================================================
        builder.Entity<BookingRequest>().HasKey(b => b.Id);
        builder.Entity<BookingRequest>().Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<BookingRequest>().Property(b => b.ClientId).IsRequired();
        builder.Entity<BookingRequest>().Property(b => b.WorkerId).IsRequired();
        builder.Entity<BookingRequest>().Property(b => b.ServiceType).IsRequired().HasMaxLength(40);
        builder.Entity<BookingRequest>().Property(b => b.Date).IsRequired();
        builder.Entity<BookingRequest>().Property(b => b.StartTime).IsRequired().HasMaxLength(5);
        builder.Entity<BookingRequest>().Property(b => b.EndTime).IsRequired().HasMaxLength(5);
        builder.Entity<BookingRequest>().Property(b => b.Hours).HasPrecision(5, 2);
        builder.Entity<BookingRequest>().Property(b => b.PaymentMethodId);
        builder.Entity<BookingRequest>().Property(b => b.Address).HasMaxLength(300);
        builder.Entity<BookingRequest>().Property(b => b.Notes).HasMaxLength(1000);
        builder.Entity<BookingRequest>().Property(b => b.HourlyRate).HasPrecision(10, 2);
        builder.Entity<BookingRequest>().Property(b => b.TotalAmount).HasPrecision(10, 2);
        builder.Entity<BookingRequest>().Property(b => b.PlatformFee).HasPrecision(10, 2);
        builder.Entity<BookingRequest>().Property(b => b.WorkerEarning).HasPrecision(10, 2);
        builder.Entity<BookingRequest>().Property(b => b.Status).IsRequired().HasMaxLength(30);
        builder.Entity<BookingRequest>().HasIndex(b => b.ClientId);
        builder.Entity<BookingRequest>().HasIndex(b => b.WorkerId);

        // ====================================================================
        // Payments
        // ====================================================================
        builder.Entity<PaymentMethod>().HasKey(p => p.Id);
        builder.Entity<PaymentMethod>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PaymentMethod>().Property(p => p.UserId).IsRequired();
        builder.Entity<PaymentMethod>().Property(p => p.Type).IsRequired().HasMaxLength(30);
        builder.Entity<PaymentMethod>().Property(p => p.Label).IsRequired().HasMaxLength(80);
        builder.Entity<PaymentMethod>().Property(p => p.Details).HasMaxLength(200);
        builder.Entity<PaymentMethod>().Property(p => p.IsDefault).HasDefaultValue(false);
        builder.Entity<PaymentMethod>().HasIndex(p => p.UserId);

        // ====================================================================
        // ReviewsAndEvaluation
        // ====================================================================
        builder.Entity<Review>().HasKey(r => r.Id);
        builder.Entity<Review>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Review>().Property(r => r.BookingId).IsRequired();
        builder.Entity<Review>().Property(r => r.ClientId).IsRequired();
        builder.Entity<Review>().Property(r => r.WorkerId).IsRequired();
        builder.Entity<Review>().Property(r => r.Rating).IsRequired();
        builder.Entity<Review>().Property(r => r.Comment).HasMaxLength(1000);
        builder.Entity<Review>().HasIndex(r => r.BookingId).IsUnique();
        builder.Entity<Review>().HasIndex(r => r.WorkerId);

        // ====================================================================
        // Messaging
        // ====================================================================
        builder.Entity<Message>().HasKey(m => m.Id);
        builder.Entity<Message>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Message>().Property(m => m.SenderId).IsRequired();
        builder.Entity<Message>().Property(m => m.RecipientId).IsRequired();
        builder.Entity<Message>().Property(m => m.Content).IsRequired().HasMaxLength(4000);
        builder.Entity<Message>().Property(m => m.ReadAt);
        builder.Entity<Message>().HasIndex(m => new { m.SenderId, m.RecipientId });
        builder.Entity<Message>().HasIndex(m => m.RecipientId);

        // Apply snake_case for the entire model
        builder.UseSnakeCaseNamingConvention();
    }
}
