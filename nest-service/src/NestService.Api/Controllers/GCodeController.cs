using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NestService.Api.Models;
using NestService.Api.Services;
using NestService.Api.ViewModels;
using System.Collections.Generic;

namespace NestService.Api.Controllers
{
    /// <summary>
    /// G-code controller.
    /// </summary>
    [Route("api/v1/g-code")]
    public class GCodeController
    {
        readonly IGCodeGenerator _gCodeGenerator;
        readonly IMapper _mapper;

        /// <summary>
        /// G-code controller constructor.
        /// </summary>
        public GCodeController(
            IGCodeGenerator gCodeGenerator,
            IMapper mapper)
        {
            _gCodeGenerator = gCodeGenerator;
            _mapper = mapper;
        }

        /// <summary>
        /// Get G-code commands.
        /// </summary>
        /// <param name="value">G-code data.</param>
        /// <returns>G-code result.</returns>
        [HttpPost]
        public ActionResult<GCodeResultViewModel> GetGCode(
            [FromBody] GCodeDataPostViewModel value)
        {
            var components = _mapper.Map<List<NestObject>>(value.Objects);
            var commands = _gCodeGenerator.GetCommands(components);
            return _mapper.Map<GCodeResultViewModel>(commands);
        }
    }
}
