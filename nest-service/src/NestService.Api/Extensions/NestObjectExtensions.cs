using NestService.Api.Models;
using NestService.Api.Models.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NestService.Api.Extensions
{
    public static class NestObjectExtensions
    {
        public static UniPath ToUniPath(this NestObject obj, NestConfig config)
        {
            var uniPath = new UniPath();
            switch (obj)
            {
                case NestGroup group:
                    {
                        var outerObj = group.Objects.First();
                        foreach (var groupObj in group.Objects)
                            if (groupObj.MaxPoint.X >= outerObj.MaxPoint.X &&
                                groupObj.MaxPoint.Y >= outerObj.MaxPoint.Y &&
                                groupObj.MinPoint.X <= outerObj.MinPoint.X &&
                                groupObj.MinPoint.Y <= outerObj.MinPoint.Y)
                                outerObj = groupObj;
                        uniPath = outerObj.ToUniPath(config);
                        foreach (var groupObj in group.Objects)
                            if (groupObj != outerObj)
                            {
                                var innerPath = groupObj.ToUniPath(config);
                                innerPath.Info = new Info { Type = groupObj.Type, Id = groupObj.Id };
                                uniPath.AddInnerPath(innerPath);
                            }
                    }
                    break;
                case NestPolygon polygon:
                    {
                        foreach (var vertex in polygon.Vertices)
                            uniPath.AddPoint(new UniPathPoint(vertex.X, vertex.Y));
                    }
                    break;
                case NestCurve curve:
                    {
                        var approxPoints = CurveApproximation(curve, config);
                        foreach (var point in approxPoints)
                            uniPath.AddPoint(point);
                    }
                    break;
            };
            uniPath.Info = new Info { Type = obj.Type, Id = obj.Id };
            return uniPath;
        }

        static List<UniPathPoint> CurveApproximation(NestCurve curve, NestConfig config)
        {
            var points = new List<UniPathPoint>();

            var a = curve.MajorRadius;
            var b = curve.MinorRadius;
            if (a != 0 && b != 0)
            {
                Line? currentLine = null, prevLine = null;
                var param = (curve.EndParam - curve.StartParam) * config.CurveApproximation;
                var currentParam = 0d;
                while (currentParam <= 2 * Math.PI)
                {
                    var arg = -Math.Tan(currentParam) * a / b;
                    var fi = Math.PI / 2 - Math.Atan(arg);
                    if (currentParam > Math.PI / 2 && currentParam <= 3 * Math.PI / 2) fi += Math.PI;
                    if (fi >= curve.StartParam && fi <= curve.EndParam)
                    {
                        var point = curve.GetPointAtParameter(fi);
                        if (currentParam == Math.PI / 2 || currentParam == 3 * Math.PI / 2) currentLine = new Line(1, 0, -point.X);
                        else currentLine = new Line(-Math.Tan(currentParam), 1, Math.Tan(currentParam) * point.X - point.Y);
                        if (prevLine is not null) points.Add(prevLine.CrossingPoint(currentLine));
                    }
                    prevLine = currentLine?.Copy();
                    currentParam += param;
                }
            }

            return points;
        }
    }
}
