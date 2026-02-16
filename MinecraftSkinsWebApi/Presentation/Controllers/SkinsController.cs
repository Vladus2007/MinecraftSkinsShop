using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkinsController : ControllerBase
    {
        private readonly ISkinService _skinService;

        public SkinsController(ISkinService skinService)
        {
            _skinService = skinService;
        }

        // GET: api/skins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkinResponse>>> GetSkins(CancellationToken ct)
        {
            // TODO: В будущем сюда стоит добавить пагинацию (page, pageSize)
            var skins = await _skinService.GetAllSkinsAsync(ct);
            return Ok(skins);
        }

        // GET: api/skins/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SkinResponse>> GetSkin(int id, CancellationToken ct)
        {
            var skin = await _skinService.GetSkinByIdAsync(id, ct);

            if (skin == null)
                return NotFound($"Skin with ID {id} not found");

            return Ok(skin);
        }
    }
}