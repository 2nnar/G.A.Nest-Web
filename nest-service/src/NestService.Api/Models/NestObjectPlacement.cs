using System;

namespace NestService.Api.Models
{
    /// <summary>
    /// Nested object placement.
    /// </summary>
    public class NestObjectPlacement
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
        public NestObjectPoint TranslationPoint { get; set; } = new();
    }
}
