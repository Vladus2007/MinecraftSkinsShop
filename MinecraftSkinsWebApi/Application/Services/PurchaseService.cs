// Application service: PurchaseService
// Responsibility:
// - Orchestrates the purchase flow: validate skin availability, compute price,
//   fetch BTC rate, create and persist Purchase entity.
// - Maps domain Purchase to PurchaseResponse DTO for the presentation layer.
// - Provides helper to check purchase ownership without exposing UserId in DTOs.

using Application.DTOs;
using Domain.Repositories;
using Application.Services.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IPurchaseService
    {
        Task<PurchaseResponse> BuySkinAsync(string userId, int skinId, CancellationToken cancellationToken);
        Task<IEnumerable<PurchaseResponse>> GetUserPurchasesAsync(string userId, CancellationToken cancellationToken);
        Task<PurchaseResponse?> GetPurchaseByIdAsync(int id, CancellationToken cancellationToken);
        Task<string?> GetPurchaseOwnerIdAsync(int id, CancellationToken cancellationToken);
    }

    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISkinRepository _skinRepository; // injected skin repository
        private readonly IPriceCalculator _priceCalculator;
        private readonly IGetRateService _rateService;

        public PurchaseService(IPurchaseRepository purchaseRepository, ISkinRepository skinRepository, IPriceCalculator priceCalculator, IGetRateService rateService)
        {
            _purchaseRepository = purchaseRepository;
            _skinRepository = skinRepository;
            _priceCalculator = priceCalculator;
            _rateService = rateService; // fix: assign to correct field
        }

        /// <summary>
        /// Execute purchase flow for given user and skin. This method handles
        /// validation, price calculation and persistence. Throws ArgumentException
        /// if skin is not available.
        /// </summary>
        public async Task<PurchaseResponse> BuySkinAsync(string userId, int skinId, CancellationToken cancellationToken)
        {
            var skin = await _skinRepository.GetSkinByIdAsync(skinId, cancellationToken);
            if (skin == null || !skin.IsAvailable)
                throw new ArgumentException("Skin not available");

            // Get BTC rate and compute final price
            var btcPrice = await _rateService.GetRateAsync(cancellationToken);
            var finalPrice = await _priceCalculator.CalculateFinalPriceAsync(skin.BasePriceUsd, cancellationToken);

            // Create domain entity and save
            var purchase = new Purchase(skin.Id, userId, finalPrice, btcPrice, DateTime.UtcNow);
            var saved = await _purchaseRepository.AddAsync(purchase, cancellationToken);

            // Map to DTO (do not include UserId)
            return new PurchaseResponse(saved.Id, saved.SkinId, saved.PaidAmountUsd, saved.BtcPriceAtMonent, saved.PurchaseAt);
        }

        /// <summary>
        /// Return purchases for the specified user.
        /// </summary>
        public async Task<IEnumerable<PurchaseResponse>> GetUserPurchasesAsync(string userId, CancellationToken cancellationToken)
        {
            var purchases = await _purchaseRepository.GetPurchasesByUserAsync(userId, cancellationToken);
            return purchases.Select(p => new PurchaseResponse(p.Id, p.SkinId, p.PaidAmountUsd, p.BtcPriceAtMonent, p.PurchaseAt));
        }

        /// <summary>
        /// Get purchase by id. Returns DTO without user information.
        /// </summary>
        public async Task<PurchaseResponse?> GetPurchaseByIdAsync(int id, CancellationToken cancellationToken)
        {
            var p = await _purchaseRepository.GetPurchaseAsync(id, cancellationToken);
            if (p == null) return null;
            return new PurchaseResponse(p.Id, p.SkinId, p.PaidAmountUsd, p.BtcPriceAtMonent, p.PurchaseAt);
        }

        /// <summary>
        /// Return owner's user id for a purchase (used by presentation layer to validate access).
        /// </summary>
        public async Task<string?> GetPurchaseOwnerIdAsync(int id, CancellationToken cancellationToken)
        {
            var p = await _purchaseRepository.GetPurchaseAsync(id, cancellationToken);
            if (p == null) return null;
            return p.UserId;
        }
    }
}
