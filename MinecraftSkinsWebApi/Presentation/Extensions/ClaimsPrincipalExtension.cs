// Presentation helper: ClaimsPrincipalExtensions
// Purpose: Provide a convenient method to extract authenticated user's string id
// from ClaimTypes.NameIdentifier. Throws UnauthorizedAccessException if claim missing.

using System;
using System.Security.Claims;

namespace Presentation.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Получить ID текущего юзера из claims как string
        public static string GetUserId(this ClaimsPrincipal user)
        {
            // Ищем ClaimTypes.NameIdentifier (стандарт для ID в .NET)
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token");

            if (string.IsNullOrEmpty(idClaim.Value))
                throw new UnauthorizedAccessException("User ID is empty in token");

            return idClaim.Value;
        }
    }
}