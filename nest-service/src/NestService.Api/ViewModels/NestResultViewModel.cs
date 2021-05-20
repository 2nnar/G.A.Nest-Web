using System;
using System.Collections.Generic;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// Nest result.
    /// </summary>
    public class NestResultViewModel
    {
        /// <summary>
        /// Nested polygon placements.
        /// </summary>
        public IEnumerable<NestObjectPlacementViewModel> Placements { get; set; } = Array.Empty<NestObjectPlacementViewModel>();
    }
}
