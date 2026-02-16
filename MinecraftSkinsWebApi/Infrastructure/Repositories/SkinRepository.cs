using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    [Obsolete("Use InMemorySkinRepository for demo purposes")]
    public class SkinRepository : ISkinRepository
    {
        private readonly ILogger<SkinRepository> _logger;
        public SkinRepository(ILogger<SkinRepository> logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<Skin>> GetAllSkinsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<Skin>>(Array.Empty<Skin>());
        }

        public Task<Skin?> GetSkinByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult<Skin?>(null);
        }
    }
}
