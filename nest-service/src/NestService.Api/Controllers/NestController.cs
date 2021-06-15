using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NestService.Api.Models;
using NestService.Api.Services;
using NestService.Api.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NestService.Api
{
    /// <summary>
    /// Nest controller.
    /// </summary>
    [Route("api/v1/nest")]
    public class NestController : Controller
    {
        readonly INester _nester;
        readonly IMapper _mapper;

        /// <summary>
        /// Nest controller constructor.
        /// </summary>
        public NestController(
            INester nester,
            IMapper mapper)
        {
            _nester = nester;
            _mapper = mapper;
        }

        /// <summary>
        /// Nest polygons.
        /// </summary>
        /// <param name="value">Nest data.</param>
        /// <param name="config">Nest config.</param>
        /// <returns>Nest result.</returns>
        [HttpPost]
        public async Task<ActionResult<NestResultViewModel>> Nest(
            [FromBody] NestDataPostViewModel value,
            [FromQuery] NestConfig config)
        {
            var configValid = IsConfigValid(config);
            if (!configValid)
                return BadRequest("Invalid config.");

            var bin = _mapper.Map<NestObject>(value.Bin);
            var components = _mapper.Map<List<NestObject>>(value.Objects);
            var placements = await _nester.GetNestedComponents(bin, components, config);
            var result = _mapper.Map<NestResultViewModel>(placements);
            return result;
        }

        bool IsConfigValid(NestConfig config)
        {
            if (config.RotationStep == 0)
                return false;

            return true;
        }
    }
}
