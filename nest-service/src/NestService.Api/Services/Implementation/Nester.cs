using NestService.Api.Extensions;
using NestService.Api.GeneticAlgorithm;
using NestService.Api.Models;
using NestService.Api.Models.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NestService.Api.Services.Implementation
{
    public class Nester : INester
    {
        static readonly TimeSpan _timeout = TimeSpan.FromMinutes(2);

        public Nester()
        {
        }

        public Task<List<NestObjectPlacement>> GetNestedComponents(NestObject binObject, List<NestObject> componentObjects, NestConfig config)
        {
            var cancellationToken = new CancellationTokenSource(_timeout).Token;

            var result = Task.Run(() =>
            {
                var bin = binObject.ToUniPath(config);
                var components = componentObjects.Select(x => x.ToUniPath(config)).ToList();

                if (config.CutThickness > 0)
                {
                    bin = bin.OffsetPolygon(-config.CutThickness / 2, config.Tolerance);
                    for (var i = 0; i < components.Count; i++)
                        components[i] = components[i].OffsetPolygon(config.CutThickness / 2, config.Tolerance);
                }
                bin.ID = -1;
                ArrangeIndexing(components);
                components = GetFittingComponents(bin, components);

                var ga = new GARunner(bin, components, config);

                var results = new List<Result>();
                for (int i = 0; i < config.IterationsCount; i++)
                {
                    var res = ga.RunAlgorithm();
                    results.Add(res);
                }

                var placements = GetPlacements(results.Where(r => r.Fitness == results.Min(x => x.Fitness)).First(), components);
                return placements;
            }, cancellationToken);

            return result;
        }

        static void ArrangeIndexing(List<UniPath> components, int initID = 0)
        {
            int index;
            for (index = initID; index < components.Count; index++)
                components[index].ID = index;
            foreach (var uniPath in components)
                foreach (var innerPath in uniPath.InnerPaths)
                    innerPath.ID = ++index;
        }

        static List<UniPath> GetFittingComponents(UniPath bin, List<UniPath> components)
        {
            var fitting = new List<UniPath>();
            foreach (var component in components)
                if (component.Width < bin.Width && component.Height < bin.Height)
                    fitting.Add(component);
            return fitting;
        }

        static List<NestObjectPlacement> GetPlacements(Result result, List<UniPath> components)
        {
            var placements = new List<NestObjectPlacement>();
            foreach (var placement in result.Placements)
            {
                var uniPath = components.Find(c => c.ID == placement.Id);
                if (uniPath is null)
                    continue;
                var pathPlacecement = new NestObjectPlacement(uniPath.Info.Id, placement.Rotation, new(placement.X, placement.Y));
                placements.Add(pathPlacecement);
            }
            return placements;
        }
    }
}
