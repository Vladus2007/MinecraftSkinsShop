using Microsoft.AspNetCore.Mvc;
using 
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MinecraftSkinsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkinsController : ControllerBase
    {
        // GET: api/<SkinsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            
        }

        // GET api/<SkinsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
           
        }

    }
