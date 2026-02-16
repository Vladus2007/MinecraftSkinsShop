// Application DTO: SkinResponse
// Purpose: Returned to clients when listing skins. Contains base price and
// calculated final price (after discounts/taxes) to avoid exposing internal logic.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record SkinResponse(
        int Id,
        string Name,
        decimal BasePriceUsd,
        decimal FinalPriceUsd,
        bool IsAvailable
        );
}
