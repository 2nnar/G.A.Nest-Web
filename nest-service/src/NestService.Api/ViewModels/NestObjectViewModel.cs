using NestService.Api.Models;
using System;

namespace NestService.Api.ViewModels
{
    /// <summary>
    /// Nest object.
    /// </summary>
    public class NestObjectViewModel
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
