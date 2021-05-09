using NestService.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NestService.Api.Services
{
    public interface INester
    {
        Task<List<NestObjectPlacement>> GetNestedComponents(NestObject bin, List<NestObject> components, NestConfig config);
    }
}
