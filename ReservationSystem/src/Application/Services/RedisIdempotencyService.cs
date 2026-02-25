using ReservationSystem.src.Application.Interfaces;
using StackExchange.Redis;

namespace ReservationSystem.src.Application.Services
{
    public class RedisIdempotencyService : IIdempotencyService
    {
        private readonly IDatabase _db;

        public RedisIdempotencyService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        private string Key(string key) => $"idempotency:{key}";

        public async Task<Guid?> GetExistingAsync(string key)
        {
            var value = await _db.StringGetAsync(Key(key));

            if (!value.HasValue)
                return null;

            return Guid.Parse(value!);
        }

        public async Task StoreAsync(string key, Guid reservationId)
        {
            await _db.StringSetAsync(
                Key(key),
                reservationId.ToString(),
                TimeSpan.FromMinutes(10) // TTL = critical
            );
        }
    }
}
