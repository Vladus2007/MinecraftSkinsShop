using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IPriceCalculator
    {
        Task<decimal> CalculateFinalPriceAsync(decimal basePriceUsd, CancellationToken cancellationToken = default);
    }
}
