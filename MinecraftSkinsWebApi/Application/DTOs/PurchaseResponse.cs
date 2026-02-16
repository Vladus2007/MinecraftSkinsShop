// Application DTO: PurchaseResponse
// Purpose: Returned to clients after a successful purchase or when querying purchases.
// Contains only non-sensitive fields. Note: UserId intentionally omitted so the API
// does not leak information about other users.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record PurchaseResponse(
        int Id,
        int SkinId,
        decimal PaidAmountUsd,
        decimal BtcPriceAtMoment,
        DateTime PurchasedAt
    );
}
