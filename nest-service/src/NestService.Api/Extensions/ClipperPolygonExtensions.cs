using ClipperLib;
using NestService.Api.Models.Geometry;
using System;
using System.Collections.Generic;

namespace NestService.Api.Extensions
{
    public static class ClipperPolygonExtensions
    {
        public static UniPath ToUniPath(this List<IntPoint> clipperPolygon, int tolerance)
        {
            var uniPath = new UniPath();
            foreach (var intPoint in clipperPolygon)
                uniPath.AddPoint(new UniPathPoint(intPoint.X / Math.Pow(10, tolerance), intPoint.Y / Math.Pow(10, tolerance)));
            return uniPath;
        }
    }
}
