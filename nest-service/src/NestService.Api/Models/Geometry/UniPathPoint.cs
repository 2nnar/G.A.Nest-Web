
namespace NestService.Api.Models.Geometry
{
    public class UniPathPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public UniPathPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
