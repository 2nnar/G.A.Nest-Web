using System;
using System.Collections.Generic;

namespace NestService.Api.ViewModels.NestObjects
{
    /// <summary>
    /// Nest group.
    /// </summary>
    public class NestGroupViewModel : NestObjectViewModel
    {
        /// <summary>
        /// List of objects.
        /// </summary>
        public IEnumerable<NestObjectViewModel> Objects { get; set; } = Array.Empty<NestObjectViewModel>();
    }
}
