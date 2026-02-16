using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Infrastructure.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId
        {
            get
            {
                // Проверяем наличие заголовка Authorization: Bearer mock-token
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == "Bearer mock-token")
                    return "test-user-1"; // фиксированный пользователь

                // В реальном проекте здесь парсинг JWT
                return "anonymous";
            }
        }
    }
}