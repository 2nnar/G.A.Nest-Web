using Microsoft.AspNetCore.Mvc;
using NestService.Api.ViewModels;
using System.Threading.Tasks;

namespace NestService.Api
{
    /// <summary>
    /// Nest controller.
    /// </summary>
    [Route("api/v1/nest")]
    public class NestController : Controller
    {
        /// <summary>
        /// Nest polygons.
        /// </summary>
        /// <param name="value">Nest data.</param>
        /// <returns>Nest result.</returns>
        [HttpPost]
        public async Task<ActionResult<NestResultViewModel>> Nest(
            [FromBody] NestDataPostViewModel value)
        {
            return NoContent();
        }
    }
}
