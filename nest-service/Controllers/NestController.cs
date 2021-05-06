using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace nest_service.Controllers
{
    [Route("api/v1/nest")]
    public class NestController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Nest()
        {
            return NoContent();
        }
    }
}
