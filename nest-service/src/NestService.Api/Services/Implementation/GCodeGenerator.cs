using NestService.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                "M3", // spindle on - clockwise
                "G21", // millimiters
                "G00 X0 Y0 Z5", // start point
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
            commands.Add("M30");
            return commands;
        }

        static IEnumerable<string> GetPolygonCommands(NestPolygon polygon)
        {
            if (!polygon.Vertices.Any())
                return Array.Empty<string>();

            var firstVertex = polygon.Vertices.First();
            var commands = new List<string>
            {
                $"(cutting polygon with ID: {polygon.Id})",
                $"G00 X{firstVertex.X} Y{firstVertex.Y} Z5".Replace(',', '.'),
                $"G01 Z-1 F100".Replace(',', '.')
            };
            foreach (var vertex in polygon.Vertices.Skip(1))
            {
                commands.Add($"G01 X{vertex.X} Y{vertex.Y} Z-1 F400".Replace(',', '.'));
            }
            commands.Add("G00 Z5");
            return commands;
        }
    }
}
