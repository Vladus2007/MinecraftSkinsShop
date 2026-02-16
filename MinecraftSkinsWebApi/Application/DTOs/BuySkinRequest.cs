// Application DTO: BuySkinRequest
// Purpose: Request body for buying a skin. Contains only the SkinId because
// the authenticated user is inferred from the request context (token/claims).

namespace Application.DTOs
{
    // UserId removed from request; user comes from token/context
    public record BuySkinRequest(int SkinId);
}
