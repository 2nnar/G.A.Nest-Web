using ClipperLib;
using NestService.Api.Extensions;
using NestService.Api.Models;
using NestService.Api.Models.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NestService.Api.GeneticAlgorithm
{
    public class Vector
    {
        public int id;
        public double X;
        public double Y;
        public double rotation;

        public Vector(double x, double y, int id, double rotation)
        {
            X = x;
            Y = y;
            this.id = id;
            this.rotation = rotation;
        }

        public override string ToString()
        {
            return "x = " + X + " , y = " + Y;
        }
    }

    public class Result
    {
        public double area;
        public double fitness;
        public List<UniPath> paths;
        public List<Vector> placements;

        public Result(List<Vector> placements, double fitness, List<UniPath> paths, double area)
        {
            this.placements = placements;
            this.fitness = fitness;
            this.paths = paths;
            this.area = area;
        }
    }

    public class GARunner
    {
        static readonly Random _rand = new();
        readonly UniPath _bin;
        readonly List<UniPath> _components;
        readonly NestConfig _config;
        Individual[] _population;

        struct Pair
        {
            public Individual male;
            public Individual female;
        }
        struct Children
        {
            public Individual firstChild;
            public Individual secondChild;
        }

        public GARunner(UniPath bin, List<UniPath> components, NestConfig config)
        {
            _config = config;
            _components = components;
            _components.Sort((x, y) =>
            {
                return x.Area.CompareTo(y.Area);
            });
            _bin = bin;
            _population = new Individual[config.PopulationSize];
            for (var i = 0; i < _population.Length; i++)
            {
                _population[i] = new Individual(_bin, _components, _config);
                _population[i].Mutate();
            }
        }

        public Result RunAlgorithm()
        {
            for (var ind = 0; ind < _population.Length; ind++)
            {
                var individual = _population[ind];
                if (individual.FitnessResult < 0)
                {
                    var rotatedPaths = new List<UniPath>();
                    foreach (var rot in individual.Rotations)
                    {
                        var rotatedPath = individual.FindPathById(rot.Key);
                        rotatedPath.Rotation = rot.Value;
                        rotatedPaths.Add(rotatedPath);
                    }

                    var nfpKeys = new List<NFP>();
                    for (var i = 0; i < rotatedPaths.Count; i++)
                    {
                        var nfp = new NFP(_bin, rotatedPaths[i], PolygonRelation.Contains, 0, rotatedPaths[i].Rotation);
                        nfpKeys.Add(nfp);
                        for (var j = 0; j < i; j++)
                        {
                            nfp = new NFP(rotatedPaths[j], rotatedPaths[i], PolygonRelation.Adjacent, rotatedPaths[j].Rotation, rotatedPaths[i].Rotation);
                            nfpKeys.Add(nfp);
                        }
                    }

                    var nfpPaths = new List<UniPath>();
                    foreach (var nfpKey in nfpKeys)
                    {
                        nfpPaths.AddRange(nfpKey.Generate(_config.Tolerance, _config.HolesUsing));
                    }

                    var res = GetNestingResult(nfpPaths, nfpKeys);
                    individual.FitnessResult = res.fitness;
                    return res;
                }
            }
            NextGeneration();
            return RunAlgorithm();
        }

        Result GetNestingResult(List<UniPath> nfpPaths, List<NFP> nfpKeys)
        {
            var paths = new List<UniPath>();
            foreach (var path in _components)
                paths.Add(path.Rotate(path.Rotation));

            var allplacements = new List<Vector>();
            var fitness = 0d;
            NFP? key = null;
            UniPath? nfp = null;

            while (paths.Count > 0)
            {
                var placed = new List<UniPath>();

                fitness += 1;
                var minwidth = double.MaxValue;
                for (var i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];
                    key = nfpKeys.Find(k => k.A.ID == _bin.ID && k.B.ID == path.ID && k.Relation == PolygonRelation.Contains);

                    if (key is null) continue;

                    var binNfp = nfpPaths[nfpKeys.IndexOf(key)];

                    var error = false;
                    for (var j = 0; j < placed.Count; j++)
                    {
                        key = nfpKeys.Find(k => k.A.ID == placed[j].ID && k.B.ID == path.ID && k.Relation == PolygonRelation.Adjacent);
                        if (key is not null)
                        {
                            nfp = nfpPaths[nfpKeys.IndexOf(key)];
                        }
                        else
                        {
                            error = true;
                            break;
                        }
                    }

                    if (error) continue;

                    Vector? position = null;
                    if (placed.Count == 0)
                    {
                        for (var k = 0; k < binNfp.Points.Count(); k++)
                            if (position is null || binNfp.GetPoint(k).X - path.GetPoint(0).X < position.X)
                                position = new Vector(
                                    binNfp.GetPoint(k).X - path.GetPoint(0).X,
                                    binNfp.GetPoint(k).Y - path.GetPoint(0).Y,
                                    path.ID,
                                    key.RotationB
                                );

                        allplacements.Add(position);
                        placed.Add(path);
                        continue;
                    }

                    var clipperBinNfp = new List<List<IntPoint>>
                    {
                        binNfp.ToClipperPolygon(_config.Tolerance)
                    };

                    var clipper = new Clipper();
                    var combinedNfp = new List<List<IntPoint>>();


                    for (var j = 0; j < placed.Count; j++)
                    {
                        key = nfpKeys.Find(k => k.A.ID == placed[j].ID && k.B.ID == path.ID && key.Relation == PolygonRelation.Adjacent);
                        nfp = nfpPaths[nfpKeys.IndexOf(key)];
                        if (nfp is null) continue;

                        var clone = nfp.ToClipperPolygon(_config.Tolerance);
                        for (var m = 0; m < clone.Count; m++)
                        {
                            var clx = clone[m].X;
                            var cly = clone[m].Y;
                            var intPoint = clone[m];
                            intPoint.X = clx + (long)(allplacements[j].X * Math.Pow(10, _config.Tolerance));
                            intPoint.Y = cly + (long)(allplacements[j].Y * Math.Pow(10, _config.Tolerance));
                            clone[m] = intPoint;
                        }

                        var areaPoly = Math.Abs(Clipper.Area(clone));
                        if (clone.Count > 2 && areaPoly > 0.1 * Math.Pow(10, _config.Tolerance) * Math.Pow(10, _config.Tolerance))
                            clipper.AddPath(clone, PolyType.ptSubject, true);
                    }

                    if (!clipper.Execute(ClipType.ctUnion, combinedNfp, PolyFillType.pftNonZero,
                        PolyFillType.pftNonZero))
                        continue;

                    var finalNfp = new List<List<IntPoint>>();
                    clipper = new Clipper();

                    clipper.AddPaths(combinedNfp, PolyType.ptClip, true);
                    clipper.AddPaths(clipperBinNfp, PolyType.ptSubject, true);
                    if (!clipper.Execute(ClipType.ctDifference, finalNfp, PolyFillType.pftNonZero,
                        PolyFillType.pftNonZero))
                        continue;

                    for (var j = 0; j < finalNfp.Count; j++)
                    {
                        var areaPoly = Math.Abs(Clipper.Area(finalNfp[j]));
                        if (finalNfp[j].Count < 3 || areaPoly < 0.1 * Math.Pow(10, _config.Tolerance) * Math.Pow(10, _config.Tolerance))
                        {
                            finalNfp.RemoveAt(j);
                            j--;
                        }
                    }

                    if (finalNfp is null || finalNfp.Count == 0) continue;

                    var f = new List<UniPath>();
                    for (var j = 0; j < finalNfp.Count; j++) f.Add(finalNfp[j].ToUniPath(_config.Tolerance));

                    var finalNfpf = f;
                    var minarea = double.MinValue;
                    var minX = double.MaxValue;
                    UniPath? nf = null;
                    var area = double.MinValue;
                    Vector? shifvector = null;
                    for (var j = 0; j < finalNfpf.Count; j++)
                    {
                        nf = finalNfpf[j];
                        if (nf.Area < 2) continue;

                        for (var k = 0; k < nf.Points.Count(); k++)
                        {
                            var allpoints = new UniPath();
                            for (var m = 0; m < placed.Count; m++)
                                for (var n = 0; n < placed[m].Points.Count(); n++)
                                    allpoints.AddPoint(new UniPathPoint(placed[m].GetPoint(n).X + allplacements[m].X,
                                        placed[m].GetPoint(n).Y + allplacements[m].Y));

                            shifvector = new Vector(nf.GetPoint(k).X - path.GetPoint(0).X, nf.GetPoint(k).Y - path.GetPoint(0).Y,
                                path.ID, key.RotationB);
                            for (var m = 0; m < path.Points.Count(); m++)
                                allpoints.AddPoint(new UniPathPoint(path.GetPoint(m).X + shifvector.X, path.GetPoint(m).Y + shifvector.Y));

                            area = allpoints.Width * 2 + allpoints.Height;
                            if (minarea == double.MinValue
                                || area < minarea
                                || Math.Abs(minarea - area) < Math.Pow(10, -_config.Tolerance)
                                && (minX == double.MinValue || shifvector.X < minX))
                            {
                                minarea = area;
                                minwidth = allpoints.Width;
                                position = shifvector;
                                minX = shifvector.X;
                            }
                        }
                    }

                    if (position is not null)
                    {
                        placed.Add(path);
                        allplacements.Add(position);
                    }
                }

                if (minwidth != double.MinValue) fitness += minwidth / _bin.Area;


                for (var i = 0; i < placed.Count; i++)
                {
                    paths.RemoveAll(p => p.ID == placed[i].ID);
                }

                if (allplacements.Count == 0) break;
            }

            fitness += 2 * paths.Count;
            return new Result(allplacements, fitness, paths, _bin.Area);
        }

        void NextGeneration()
        {
            var nextgen = new Individual[_population.Length];
            _population = _population.OrderByDescending(x => x.FitnessResult).ToArray();
            nextgen[0] = _population[0];
            for (var i = 1; i < nextgen.Length; i += 2)
            {
                var parents = RouletteParentsSelection();
                while (parents.male is null || parents.female is null) parents = RouletteParentsSelection();
                var children = SinglePointCrossover(parents);
                nextgen[i] = children.firstChild;
                if (i + 1 < nextgen.Length) nextgen[i + 1] = children.secondChild;
            }
            _population = nextgen;
        }

        Pair RouletteParentsSelection()
        {
            var pair = new Pair();
            var male_index = -1;
            var probs = new double[_population.Length];
            for (var i = 0; i < probs.Length; i++)
                probs[i] = Math.Round(_population[i].FitnessResult, 2);
            var sum = probs.Sum();
            for (var i = 0; i < probs.Length; i++)
                probs[i] /= sum;
            double begin;
            var end = 0d;
            var randomProb = _rand.NextDouble();
            for (int i = 0; i < probs.Length; i++)
            {
                begin = end;
                end += probs[i];
                if (randomProb == 0) { pair.male = _population[0]; male_index = 0; }
                else if (begin < randomProb && randomProb <= end) { pair.male = _population[i]; male_index = i; break; }
            }

            end = 0;
            randomProb = _rand.NextDouble();
            for (var i = 0; i < probs.Length; i++)
            {
                begin = end;
                end += probs[i];
                if (i != male_index)
                {
                    if (randomProb == 0) pair.female = _population[0];
                    else if (begin < randomProb && randomProb <= end) { pair.female = _population[i]; break; }
                }
            }
            return pair;
        }

        Children SinglePointCrossover(Pair pair)
        {
            var children = new Children();
            var inheritedGene1 = new List<UniPath>();
            var inheritedGene2 = new List<UniPath>();
            var ids1 = new List<int>();
            var rotations1 = new Dictionary<int, double>();
            var ids2 = new List<int>();
            var rotations2 = new Dictionary<int, double>();

            var crossoverPoint = _rand.Next(1, _components.Count - 1);
            for (var i = 0; i < crossoverPoint; i++)
            {
                ids1.Add(pair.male.GetPath(i).ID);
                rotations1[pair.male.GetPath(i).ID] = pair.male.Rotations[pair.male.GetPath(i).ID];
                ids2.Add(pair.female.GetPath(i).ID);
                rotations2[pair.female.GetPath(i).ID] = pair.female.Rotations[pair.female.GetPath(i).ID];
            }
            for (var i = 0; i < _components.Count; i++)
            {
                if (!ids1.Contains(pair.female.GetPath(i).ID))
                {
                    ids1.Add(pair.female.GetPath(i).ID);
                    rotations1[pair.female.GetPath(i).ID] = pair.female.Rotations[pair.female.GetPath(i).ID];
                }
                if (!ids2.Contains(pair.male.GetPath(i).ID))
                {
                    ids2.Add(pair.male.GetPath(i).ID);
                    rotations2[pair.male.GetPath(i).ID] = pair.male.Rotations[pair.male.GetPath(i).ID];
                }
            }
            for (var i = 0; i < crossoverPoint; i++)
            {
                inheritedGene1.Add(pair.male.FindPathById(ids1.ToArray()[i]));
                inheritedGene2.Add(pair.female.FindPathById(ids2.ToArray()[i]));
            }
            for (var i = crossoverPoint; i < _components.Count; i++)
            {
                inheritedGene1.Add(pair.female.FindPathById(ids1.ToArray()[i]));
                inheritedGene2.Add(pair.male.FindPathById(ids2.ToArray()[i]));
            }

            var firstChild = new Individual(_bin, inheritedGene1, _config) { Rotations = rotations1 };
            var secondChild = new Individual(_bin, inheritedGene2, _config) { Rotations = rotations2 };
            children.firstChild = firstChild;
            children.secondChild = secondChild;
            return children;
        }
    }
}
