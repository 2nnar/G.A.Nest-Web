using NestService.Api.ViewModels.Abstract;

namespace NestService.Api.ViewModels.NestObjects
{
    /// <summary>
    /// Nest curve.
    /// </summary>
    public class NestCurveViewModel : NestObjectViewModel
    {
        /// <summary>
        /// Center point.
        /// </summary>
        public NestObjectVertexViewModel Center { get; set; } = new();

        /// <summary>
        /// Major radius.
        /// </summary>
        public double MajorRadius { get; set; }

        /// <summary>
        /// Minor radius.
        /// </summary>
        public double MinorRadius { get; set; }

        /// <summary>
        /// Start parameter.
        /// </summary>
        public double StartParam { get; set; }

        /// <summary>
        /// End parameter.
        /// </summary>
        public double EndParam { get; set; }
    }
}
