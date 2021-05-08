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

        /// <summary>
        /// Up and rightmost point.
        /// </summary>
        public abstract NestObjectPoint MaxPoint { get; }

        /// <summary>
        /// Down and leftmost point.
        /// </summary>
        public abstract NestObjectPoint MinPoint { get; }
    }
}
