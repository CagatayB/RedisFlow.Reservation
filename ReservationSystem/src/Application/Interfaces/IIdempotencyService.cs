namespace ReservationSystem.src.Application.Interfaces
{
    public interface IIdempotencyService
    {
        Task<Guid?> GetExistingAsync(string key);
        Task StoreAsync(string key, Guid reservationId);
    }
}
