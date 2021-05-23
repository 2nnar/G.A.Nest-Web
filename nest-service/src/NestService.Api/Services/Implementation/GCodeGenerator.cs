using NestService.Api.Models;
using System;
using System.Collections.Generic;

namespace NestService.Api.Services.Implementation
{
    public class GCodeGenerator : IGCodeGenerator
    {
        /// <summary>
        /// Get G-code commands for nested objects.
        /// </summary>
        /// <param name="objects">Nested objects.</param>
        /// <returns>List of commands.</returns>
        public IEnumerable<string> GetCommands(IEnumerable<NestObject> objects)
        {
            var commands = new List<string>()
            {
                "G00 X0 Y0",
            };

            foreach (var obj in objects)
            {
                var objCommands = obj switch
                {
                    NestPolygon polygon => GetPolygonCommands(polygon),
                    _ => Array.Empty<string>() // TODO: curve and group commands
                };
                commands.AddRange(objCommands);
            }

            commands.Add("G00 X0 Y0");
            return commands;
        }

        static IEnumerable<string> GetPolygonCommands(NestPolygon polygon)
        {
            var commands = new List<string>();
            foreach (var vertex in polygon.Vertices)
            {
                commands.Add($"G00 X{vertex.X} Y{vertex.Y}".Replace(',', '.'));
            }
            return commands;
        }
    }
}
