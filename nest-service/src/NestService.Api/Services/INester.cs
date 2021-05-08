using NestService.Api.Models;
using NestService.Api.Models.Geometry;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NestService.Api.Services
{
    public interface INester
    {
        Task<List<Placement>> GetNestedComponents(UniPath bin, List<UniPath> components, NestConfig config);
    }
}
