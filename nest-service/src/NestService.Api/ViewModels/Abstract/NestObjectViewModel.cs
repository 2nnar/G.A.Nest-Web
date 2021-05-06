using NestService.Api.Models;
using System;

namespace NestService.Api.ViewModels.Abstract
{
    /// <summary>
    /// Nest object.
    /// </summary>
    public abstract class NestObjectViewModel
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
