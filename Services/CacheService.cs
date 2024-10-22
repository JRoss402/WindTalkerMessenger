using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;

        public CacheService(IMemoryCache cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public async Task<List<Message>> CacheChats()
        {
            var cacheKey = "chats";


            if (!_cache.TryGetValue(cacheKey, out List<Message> messages))
            {
                var chats = await _context.Chats.ToListAsync();
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(300));


                _cache.Set(cacheKey, chats, cacheOptions);
            }

            return messages;
        }
    }
}
