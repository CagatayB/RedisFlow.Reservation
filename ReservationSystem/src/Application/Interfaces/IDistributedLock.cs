namespace ReservationSystem.src.Application.Interfaces
{
    public interface IDistributedLock
    {
        Task<bool> AcquireAsync(string key, TimeSpan expiry);
        Task ReleaseAsync(string key);
    }
}
