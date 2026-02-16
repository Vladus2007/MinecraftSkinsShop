// Application service interface: IGetRateService
// Purpose: Abstraction for obtaining current BTC/USD price. Implementations
// may call external APIs, use caching, and support CancellationToken.

using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IGetRateService
    {
        Task<decimal> GetRateAsync(CancellationToken cancellationToken = default);
    }
}
