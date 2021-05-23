using NestService.Api.Models;
using System.Collections.Generic;

namespace NestService.Api.Services
{
    public interface IGCodeGenerator
    {
        /// <summary>
        /// Get G-code commands for nested objects.
        /// </summary>
        /// <param name="objects">Nested objects.</param>
        /// <returns>List of commands.</returns>
        IEnumerable<string> GetCommands(IEnumerable<NestObject> objects);
    }
}
