using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public interface ICacheService
    {
        Task<List<Message>> CacheChats();

    }
}
