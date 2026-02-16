using Domain.Models;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EfPurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext _db;

        public EfPurchaseRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Purchase?> GetPurchaseAsync(int id, CancellationToken _token)
        {
            return await _db.Purchases.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, _token);
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByUserAsync(string userId, CancellationToken _token)
        {
            return await _db.Purchases.AsNoTracking().Where(p => p.UserId == userId).ToListAsync(_token);
        }

        public async Task<Purchase> AddAsync(Purchase purchase, CancellationToken cancellationToken)
        {
            _db.Purchases.Add(purchase);
            await _db.SaveChangesAsync(cancellationToken);
            return purchase;
        }
    }
}
