using ReservationSystem.src.Application.Commands;

namespace ReservationSystem.src.Application.Interfaces
{
    public interface IReservationService
    {
        Task<Guid> CreateReservationAsync(CreateReservationCommand command);
    }
}
