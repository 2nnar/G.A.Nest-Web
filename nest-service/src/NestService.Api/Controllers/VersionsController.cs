using Microsoft.AspNetCore.Mvc;
using NestService.Api.Configuration;
using System.Threading.Tasks;

namespace NestService.Api.Controllers
{
    /// <summary>
    /// Versions controller.
    /// </summary>
    [Route("api/versions")]
    public class VersionsController : Controller
    {
        /// <summary>
        /// Get current version.
        /// </summary>
        /// <returns>Version model.</returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var version = new
            {
                AppConstants.Version
            };
            return Ok(version);
        }
    }
}
