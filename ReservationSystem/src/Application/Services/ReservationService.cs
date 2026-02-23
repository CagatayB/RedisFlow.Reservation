using Microsoft.EntityFrameworkCore;
using ReservationSystem.src.Application.Commands;
using ReservationSystem.src.Application.Interfaces;
using ReservationSystem.src.Domain.Entities;
using ReservationSystem.src.Domain.Enums;
using ReservationSystem.src.Infrastructure.Persistence;

namespace ReservationSystem.src.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IDistributedLock _lock;

        public ReservationService(AppDbContext context, IDistributedLock @lock)
        {
            _context = context;
            _lock = @lock;
        }

        public async Task<Guid> CreateReservationAsync(CreateReservationCommand command)
        {
            var lockKey = $"lock:slot:{command.SlotId}";

            var acquired = await _lock.AcquireAsync(lockKey, TimeSpan.FromSeconds(5));

            if (!acquired)
                throw new Exception("Slot is currently being reserved");

            try
            {
                var slot = await _context.Slots.FindAsync(command.SlotId);

                if (slot == null)
                    throw new Exception("Slot not found");

                var activeReservations = await _context.Reservations
                    .CountAsync(x => x.SlotId == command.SlotId && x.Status == ReservationStatus.Active);

                if (activeReservations >= slot.Capacity)
                    throw new Exception("Slot full");

                var reservation = new Reservation
                {
                    Id = Guid.NewGuid(),
                    SlotId = command.SlotId,
                    UserId = command.UserId,
                    Status = ReservationStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reservations.Add(reservation);

                await _context.SaveChangesAsync();

                return reservation.Id;
            }
            finally
            {
                await _lock.ReleaseAsync(lockKey);
            }
        }
    }
}
