using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseResponse> BuySkinAsync(string userId, int skinId, CancellationToken cancellationToken);
        Task<IEnumerable<PurchaseResponse>> GetUserPurchasesAsync(string userId, CancellationToken cancellationToken);
        Task<PurchaseResponse?> GetPurchaseByIdAsync(int id, CancellationToken cancellationToken);
        Task<string?> GetPurchaseOwnerIdAsync(int id, CancellationToken cancellationToken);
    }
}
