namespace ReservationSystem.src.Application.Interfaces
{
    public interface IAvailabilityCache
    {
        Task<int?> GetRemainingCapacityAsync(Guid slotId);
        Task SetRemainingCapacityAsync(Guid slotId, int remaining);
        Task InvalidateAsync(Guid slotId);
    }
}
