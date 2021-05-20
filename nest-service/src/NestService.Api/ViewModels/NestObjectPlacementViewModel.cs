using System;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// Nested object placement.
    /// </summary>
    public class NestObjectPlacementViewModel
    {
        /// <summary>
        /// Object ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Object rotation.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Object translation point.
        /// </summary>
        public NestObjectPointViewModel TranslationPoint { get; set; } = new();
    }
}
