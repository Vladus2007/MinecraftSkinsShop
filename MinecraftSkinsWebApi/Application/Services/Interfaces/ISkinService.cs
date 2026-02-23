using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ISkinService
    {
        Task<IEnumerable<SkinResponse>> GetAllSkinsAsync(CancellationToken cancellationToken);
        Task<SkinResponse?> GetSkinByIdAsync(int id, CancellationToken cancellationToken);
    }
}
