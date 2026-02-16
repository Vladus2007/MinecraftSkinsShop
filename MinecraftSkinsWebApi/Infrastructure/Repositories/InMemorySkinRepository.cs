using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading;

namespace Infrastructure.Repositories
{
    public class InMemorySkinRepository : ISkinRepository
    {
        private readonly ConcurrentDictionary<int, Skin> _skins = new();
        private readonly ILogger<InMemorySkinRepository> _logger;

        public InMemorySkinRepository(ILogger<InMemorySkinRepository> logger)
        {
            _logger = logger;
            // seed
            var s1 = new Skin(1, "Default Skin", 2.50m, true);
            var s2 = new Skin(2, "Rare Skin", 10m, true);
            _skins[s1.Id] = s1;
            _skins[s2.Id] = s2;
        }

        public Task<IEnumerable<Skin>> GetAllSkinsAsync(CancellationToken _token)
        {
            return Task.FromResult(_skins.Values.AsEnumerable());
        }

        public Task<Skin?> GetSkinByIdAsync(int id, CancellationToken _token)
        {
            _skins.TryGetValue(id, out var skin);
            return Task.FromResult<Skin?>(skin);
        }
    }
}
