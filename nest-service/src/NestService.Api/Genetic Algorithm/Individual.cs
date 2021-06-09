using NestService.Api.Models;
using NestService.Api.Models.Geometry;
using System;
using System.Collections.Generic;

namespace NestService.Api.GeneticAlgorithm
{
    public class Individual
    {
        static readonly Random _rand = new();
        readonly UniPath _bin;
        readonly List<UniPath> _paths;
        readonly NestConfig _config;

        public Dictionary<int, double> Rotations { get; set; }
        public double FitnessResult { get; set; }
        public IEnumerable<UniPath> Paths
        {
            get
            {
                foreach (UniPath p in _paths)
                    yield return p;
            }
        }

        public Individual(UniPath bin, List<UniPath> uniPaths, NestConfig config)
        {
            FitnessResult = -1;
            _bin = bin;
            _config = config;
            _paths = new List<UniPath>();
            Rotations = new Dictionary<int, double>();
            _paths.AddRange(uniPaths);
            foreach (var p in _paths)
                SetRandomRotation(p);
        }

        public UniPath GetPath(int i)
        {
            return _paths[i];
        }

        public UniPath FindPathById(int id)
        {
            var path = _paths.Find(x => x.ID == id);
            if (path is null)
                throw new Exception("Path not found.");
            return path;
        }

        void SetRandomRotation(UniPath path)
        {
            var angle = _rand.NextDouble() * 2 * Math.PI;
            var rotatedPath = path.Rotate(angle);
            if (rotatedPath.Width < _bin.Width && rotatedPath.Height < _bin.Height)
                Rotations[path.ID] = angle;
            else SetRandomRotation(path);
        }

        public void Mutate()
        {
            for (var i = 0; i < _paths.Count; i++)
            {
                if (_rand.NextDouble() <= _config.MutationRate)
                {
                    var j = (i + 1) % _paths.Count;
                    UniPath temp = _paths[i];
                    _paths[i] = _paths[j];
                    _paths[j] = temp;
                }
                if (_rand.NextDouble() <= _config.MutationRate) SetRandomRotation(_paths[i]);
            }
        }
    }
}
