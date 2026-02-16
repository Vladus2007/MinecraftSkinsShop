using Domain.Models;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EfSkinRepository : ISkinRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<EfSkinRepository> _logger;

        public EfSkinRepository(AppDbContext db, ILogger<EfSkinRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Skin>> GetAllSkinsAsync(CancellationToken cancellationToken)
        {
            return await _db.Skins.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Skin?> GetSkinByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _db.Skins.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
    }
}
