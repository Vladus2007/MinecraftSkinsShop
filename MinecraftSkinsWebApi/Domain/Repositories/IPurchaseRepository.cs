using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase?> GetPurchaseAsync(int id, CancellationToken _token);

        Task<IEnumerable<Purchase>> GetPurchasesByUserAsync(string userId, CancellationToken _token);

        Task<Purchase> AddAsync(Purchase purchase, CancellationToken cancellationToken);
    }
}
