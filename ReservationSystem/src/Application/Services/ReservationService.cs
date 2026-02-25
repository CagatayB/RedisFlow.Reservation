using Microsoft.EntityFrameworkCore;
using ReservationSystem.src.Application.Commands;
using ReservationSystem.src.Application.Interfaces;
using ReservationSystem.src.Domain.Entities;
using ReservationSystem.src.Domain.Enums;
using ReservationSystem.src.Infrastructure.Persistence;

namespace ReservationSystem.src.Application.Services
{
    public class ReservationService
    {
        private readonly AppDbContext _context;
        private readonly IDistributedLock _lock;
        private readonly IAvailabilityCache _cache;
        private readonly IIdempotencyService _idempotency;

        public ReservationService(
            AppDbContext context,
            IDistributedLock @lock,
            IAvailabilityCache cache,
            IIdempotencyService idempotency)
        {
            _context = context;
            _lock = @lock;
            _cache = cache;
            _idempotency = idempotency;
        }

        public async Task<Guid> CreateReservationAsync(
            CreateReservationCommand command,
            string idempotencyKey)
        {
            var existing = await _idempotency.GetExistingAsync(idempotencyKey);
            if (existing.HasValue) return existing.Value;

            var lockKey = $"lock:slot:{command.SlotId}";
            var acquired = await _lock.AcquireAsync(lockKey, TimeSpan.FromSeconds(5));

            if (!acquired)
                throw new Exception("Slot busy");

            try
            {
                var slot = await _context.Slots.FindAsync(command.SlotId);
                if (slot == null) throw new Exception("Slot not found");

                var cachedRemaining = await _cache.GetRemainingCapacityAsync(slot.Id);

                int remaining;

                if (cachedRemaining.HasValue)
                {
                    remaining = cachedRemaining.Value;
                }
                else
                {
                    var activeReservations = await _context.Reservations
                        .CountAsync(x => x.SlotId == slot.Id &&
                                         x.Status == ReservationStatus.Active);

                    remaining = slot.Capacity - activeReservations;

                    await _cache.SetRemainingCapacityAsync(slot.Id, remaining);
                }

                if (remaining <= 0)
                    throw new Exception("Slot full");

                var reservation = new Reservation
                {
                    Id = Guid.NewGuid(),
                    SlotId = slot.Id,
                    UserId = command.UserId,
                    Status = ReservationStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                await _idempotency.StoreAsync(idempotencyKey, reservation.Id);
                await _cache.InvalidateAsync(slot.Id);

                return reservation.Id;
            }
            finally
            {
                await _lock.ReleaseAsync(lockKey);
            }
        }
    }
}
