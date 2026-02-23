using ReservationSystem.src.Application.Interfaces;
using StackExchange.Redis;

namespace ReservationSystem.src.Application.Commands
{
    public class RedisDistributedLock : IDistributedLock
    {
        private readonly IDatabase _db;

        public RedisDistributedLock(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<bool> AcquireAsync(string key, TimeSpan expiry)
        {
            return await _db.StringSetAsync(
                key,
                Environment.MachineName,
                expiry,
                When.NotExists
            );
        }

        public async Task ReleaseAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
