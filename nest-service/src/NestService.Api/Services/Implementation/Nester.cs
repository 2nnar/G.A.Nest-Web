using NestService.Api.GeneticAlgorithm;
using NestService.Api.Models;
using NestService.Api.Models.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestService.Api.Services.Implementation
{
    public class Nester : INester
    {
        public Nester()
        {
        }

        public Task<List<Placement>> GetNestedComponents(UniPath bin, List<UniPath> components, NestConfig config)
        {
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

            return Task.FromResult(GetPlacements(results.Where(r => r.fitness == results.Min(x => x.fitness)).First(), components));
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

        static List<Placement> GetPlacements(Result result, List<UniPath> components)
        {
            var placements = new List<Placement>();
            foreach (var placement in result.placements)
            {
                var uniPath = components.Find(c => c.ID == placement.id);
                var pathPlacecement = new Placement(uniPath, placement.rotation, new UniPathPoint(placement.X, placement.Y));
                placements.Add(pathPlacecement);
            }
            return placements;
        }
    }
}
