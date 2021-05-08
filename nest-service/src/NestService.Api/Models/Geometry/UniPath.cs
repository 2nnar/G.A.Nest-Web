using ClipperLib;
using NestService.Api.Extensions;
using NfpLib.Data;
using System;
using System.Collections.Generic;

namespace NestService.Api.Models.Geometry
{
    public struct Info
    {
        public Guid Id;
        public NestObjectType Type;
        public override string ToString()
        {
            if (Id == Guid.Empty && Type == NestObjectType.Unknown) return "Undefined";
            else return $"{Type} {Id}";
        }
    }

    public class UniPath
    {
        readonly List<UniPath> _innerPaths;
        readonly List<UniPathPoint> _points;

        public IEnumerable<UniPath> InnerPaths
        {
            get
            {
                foreach (var uniPath in _innerPaths)
                    yield return uniPath;
            }
        }
        public IEnumerable<UniPathPoint> Points
        {
            get
            {
                foreach (var point in _points)
                    yield return point;
            }
        }
        public int ID { get; set; }
        public Info Info { get; set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public double Area { get; private set; }
        public double Rotation { get; set; }

        public UniPath()
        {
            _innerPaths = new List<UniPath>();
            _points = new List<UniPathPoint>();
            Info = new Info() { Id = Guid.Empty, Type = NestObjectType.Unknown };
        }

        public void AddPoint(UniPathPoint point)
        {
            _points.Add(point);
            var min_x = _points[0].X;
            var min_y = _points[0].Y;
            var max_x = _points[0].X;
            var max_y = _points[0].Y;
            for (int i = 1; i < _points.Count; i++)
            {
                var x = _points[i].X;
                var y = _points[i].Y;
                if (x < min_x) min_x = x;
                if (y < min_y) min_y = y;
                if (x > max_x) max_x = x;
                if (y > max_y) max_y = y;
            }
            Width = max_x - min_x;
            Height = max_y - min_y;
            if (_points.Count > 2)
            {
                Area = 0;
                for (var i = 0; i < _points.Count; i++)
                {
                    var j = (i + 1) % _points.Count;
                    Area += _points[i].X * _points[j].Y - _points[i].Y * _points[j].X;
                }
                Area = Math.Abs(Area / 2);
            }
        }

        public UniPathPoint GetPoint(int i)
        {
            return _points[i];
        }

        public void AddInnerPath(UniPath innerPath)
        {
            _innerPaths.Add(innerPath);
        }

        public void ClearInnerPaths()
        {
            _innerPaths.Clear();
        }

        public void ClearPoints()
        {
            _points.Clear();
        }

        public override string ToString()
        {
            var res = $"Info: {Info}" + Environment.NewLine;
            for (var i = 0; i < _points.Count; i++)
                res += $"Point{i}:({_points[i].X};{_points[i].Y}) ";
            res += Environment.NewLine;
            if (_innerPaths.Count > 0) res += "Inner paths count = " + _innerPaths.Count;
            return res;
        }

        public List<IntPoint> ToClipperPolygon(int tolerance)
        {
            var path = new List<IntPoint>();
            foreach (var point in Points)
                path.Add(new IntPoint((long)(point.X * Math.Pow(10, tolerance)), (long)(point.Y * Math.Pow(10, tolerance))));
            return path;
        }

        public NestPath ToNestPath()
        {
            var outerPath = new NestPath();
            foreach (var point in _points)
                outerPath.add(point.X, point.Y);
            outerPath.area = Area;
            outerPath.bid = ID + 1;
            outerPath.setRotation(Rotation);
            outerPath.setId(ID);
            foreach (var innerPath in _innerPaths)
            {
                var innerNestPath = innerPath.ToNestPath();
                outerPath.getChildren().Add(innerNestPath);
                innerNestPath.setParent(outerPath);
            }
            return outerPath;
        }

        public UniPath Rotate(double angle)
        {
            angle %= 2 * Math.PI;
            var rotatedPath = Copy();
            rotatedPath.ClearPoints();
            foreach (var point in _points)
            {
                var x = point.X;
                var y = point.Y;
                var rot_x = x * Math.Cos(angle) - y * Math.Sin(angle);
                var rot_y = x * Math.Sin(angle) + y * Math.Cos(angle);
                rotatedPath.AddPoint(new UniPathPoint(rot_x, rot_y));
            }
            rotatedPath.ClearInnerPaths();
            foreach (var innerPath in InnerPaths)
                rotatedPath.AddInnerPath(innerPath.Rotate(angle));
            return rotatedPath;
        }

        public UniPath Copy()
        {
            var copy = new UniPath();
            foreach (var point in Points)
                copy.AddPoint(point);
            foreach (var innerPath in InnerPaths)
                copy.AddInnerPath(innerPath);
            copy.ID = ID;
            copy.Info = Info;
            copy.Rotation = Rotation;
            return copy;
        }

        public UniPath OffsetPolygon(double offset, int tolerance)
        {
            var clipperOffset = new ClipperOffset(2, 0.25 * Math.Pow(10, tolerance));
            clipperOffset.AddPath(ToClipperPolygon(tolerance), JoinType.jtRound, EndType.etClosedPolygon);
            var offsetedClipperPolygon = new List<List<IntPoint>>();
            clipperOffset.Execute(ref offsetedClipperPolygon, offset * Math.Pow(10, tolerance));
            var uniPath = Copy();
            uniPath.ClearPoints();
            uniPath.ClearInnerPaths();
            foreach (var point in offsetedClipperPolygon[0].ToUniPath(tolerance).Points)
                uniPath.AddPoint(point);
            if (_innerPaths.Count > 0)
                foreach (var innerPath in _innerPaths)
                    uniPath.AddInnerPath(innerPath.OffsetPolygon(-offset, tolerance));
            return uniPath;
        }
    }
}
