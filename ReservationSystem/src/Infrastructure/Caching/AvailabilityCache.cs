using ReservationSystem.src.Application.Interfaces;
using StackExchange.Redis;

namespace ReservationSystem.src.Infrastructure.Caching
{
    public class AvailabilityCache : IAvailabilityCache
    {
        private readonly IDatabase _db;

        public AvailabilityCache(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        private string Key(Guid slotId) => $"availability:slot:{slotId}";

        public async Task<int?> GetRemainingCapacityAsync(Guid slotId)
        {
            var value = await _db.StringGetAsync(Key(slotId));

            if (!value.HasValue)
                return null;

            return (int)value;
        }

        public async Task SetRemainingCapacityAsync(Guid slotId, int remaining)
        {
            await _db.StringSetAsync(Key(slotId), remaining, TimeSpan.FromMinutes(5));
        }

        public async Task InvalidateAsync(Guid slotId)
        {
            await _db.KeyDeleteAsync(Key(slotId));
        }
    }
}
