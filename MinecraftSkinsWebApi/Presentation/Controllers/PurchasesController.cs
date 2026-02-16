// Presentation layer: PurchasesController
// Responsibilities:
// - Expose HTTP endpoints related to purchases (buy skin, list user's purchases, get purchase by id)
// - Enforce authorization on all endpoints ([Authorize])
// - Extract authenticated user id from ClaimsPrincipal using Presentation.Extensions.ClaimsPrincipalExtensions
// - Delegate business logic to IPurchaseService (application layer)
// - Return DTOs (PurchaseResponse) to the client and avoid exposing internal fields like UserId

using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions; // Helper extension method to obtain user id from ClaimsPrincipal

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all actions in this controller
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        // Constructor: controller gets IPurchaseService injected via DI
        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Buy a skin for the current authenticated user.
        /// Request body: BuySkinRequest (contains SkinId only). UserId is taken from token.
        /// Returns: 201 Created with PurchaseResponse (does not contain UserId).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PurchaseResponse>> Buy(
            [FromBody] BuySkinRequest request,
            CancellationToken ct)
        {
            // Get user id (string) from claims (throws UnauthorizedAccessException if missing)
            var userId = User.GetUserId();

            // Delegate to application service which performs validation, pricing, rate lookup and persistence
            var purchase = await _purchaseService.BuySkinAsync(userId, request.SkinId, ct);

            // Return created response pointing to GetPurchaseById
            return CreatedAtAction(
                nameof(GetPurchaseById),
                new { id = purchase.Id },
                purchase
            );
        }

        /// <summary>
        /// Get current user's purchase history.
        /// Route: GET /api/purchases/history
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<PurchaseResponse>>> GetMyPurchases(CancellationToken ct)
        {
            // Extract current user id (string) from token
            var userId = User.GetUserId();

            // Service returns purchases filtered by user
            var purchases = await _purchaseService.GetUserPurchasesAsync(userId, ct);

            return Ok(purchases);
        }

        /// <summary>
        /// Get a specific purchase by id. Ensures requester is the owner.
        /// Route: GET /api/purchases/{id}
        /// Security: only owner can read their purchase; otherwise returns 403 Forbidden.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PurchaseResponse>> GetPurchaseById(int id, CancellationToken ct)
        {
            var userId = User.GetUserId();

            // Get purchase DTO (does not include UserId to the client)
            var purchase = await _purchaseService.GetPurchaseByIdAsync(id, ct);

            if (purchase == null)
                return NotFound();

            // Ensure the authenticated user is the owner of the purchase
            // Ownership check uses a separate service call to avoid leaking UserId in DTO
            var ownerId = await _purchaseService.GetPurchaseOwnerIdAsync(id, ct);
            if (ownerId == null || ownerId != userId)
                return Forbid(); // Or return NotFound() to hide existence

            return Ok(purchase);
        }
    }
}