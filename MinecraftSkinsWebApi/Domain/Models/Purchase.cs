using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int SkinId { get; set; }
        public string UserId { get; set; }

        public decimal PaidAmountUsd { get; set; }
        public decimal BtcPriceAtMonent { get; set; }
        public DateTime PurchaseAt { get; set; }

        private Purchase() { }

        public Purchase(int skinId, string userId, decimal paidAmountUsd, decimal btcPriceAtMonent, DateTime purchaseAt)
        {
            SkinId = skinId;
            UserId = userId;
            PaidAmountUsd = paidAmountUsd;
            BtcPriceAtMonent = btcPriceAtMonent;
            PurchaseAt = purchaseAt;
        }
    }
}
