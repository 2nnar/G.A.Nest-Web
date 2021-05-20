using NestService.Api.Models.Geometry;
using NfpLib.Data;
using System.Collections.Generic;

namespace NestService.Api.Extensions
{
    public static class NestPathExtensions
    {
        public static UniPath ToUniPath(this NestPath nestPath)
        {
            var uniPath = new UniPath();
            foreach (var point in nestPath.getSegments())
                uniPath.AddPoint(new UniPathPoint(point.x, point.y));
            uniPath.ID = nestPath.getId();
            foreach (var child in nestPath.getChildren())
            {
                var innerPath = child.ToUniPath();
                uniPath.AddInnerPath(innerPath);
            }
            return uniPath;
        }

        public static List<UniPath> ToUniPaths(this List<NestPath> nestPaths)
        {
            var uniPaths = new List<UniPath>();
            foreach (var nestPath in nestPaths)
                uniPaths.Add(nestPath.ToUniPath());
            return uniPaths;
        }
    }
}
