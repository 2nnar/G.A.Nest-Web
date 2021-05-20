namespace NestService.Api.Models
{
    /// <summary>
    /// Nest object point.
    /// </summary>
    public class NestObjectPoint
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NestObjectPoint()
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public NestObjectPoint(double x, double y)
            => (X, Y) = (x, y);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public NestObjectPoint(double x, double y, double z)
            => (X, Y, Z) = (x, y, z);
    }
}
