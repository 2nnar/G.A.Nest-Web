
namespace NestService.Api.Models.Geometry
{
    public class Line
    {
        public struct LineCoefficients
        {
            public double A;
            public double B;
            public double C;
        }

        public LineCoefficients Coeffs { get; set; }

        public Line(double a, double b, double c)
        {
            Coeffs = new LineCoefficients() { A = a, B = b, C = c };
        }

        public UniPathPoint CrossingPoint(Line line)
        {
            var d = Coeffs.A * line.Coeffs.B - Coeffs.B * line.Coeffs.A;
            var dx = -Coeffs.C * line.Coeffs.B + Coeffs.B * line.Coeffs.C;
            var dy = -Coeffs.A * line.Coeffs.C + Coeffs.C * line.Coeffs.A;
            return new UniPathPoint(dx / d, dy / d);
        }

        public Line Copy()
        {
            return new Line(Coeffs.A, Coeffs.B, Coeffs.C);
        }
    }
}
