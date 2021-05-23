using System;
using System.Collections.Generic;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// G-code generation result.
    /// </summary>
    public class GCodeResultViewModel
    {
        /// <summary>
        /// List of commands.
        /// </summary>
        public IEnumerable<string> Commands { get; set; } = Array.Empty<string>();
    }
}
