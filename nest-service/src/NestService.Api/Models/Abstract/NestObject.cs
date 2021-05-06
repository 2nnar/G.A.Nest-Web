using System;

namespace NestService.Api.Models.Abstract
{
    /// <summary>
    /// Nest object.
    /// </summary>
    public abstract class NestObject
    {
        /// <summary>
        /// Object ID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Object type.
        /// </summary>
        public NestObjectType Type { get; set; }
    }
}
