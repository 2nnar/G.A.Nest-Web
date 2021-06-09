
namespace NestService.Api.Models
{
    public class NestConfig
    {
        public int Tolerance { get; set; } = 3;
        public double CutThickness { get; set; } = 0;
        public double CurveApproximation { get; set; } = 0.05;
        public int RotationStep { get; set; } = 90;

        public int IterationsCount { get; set; } = 1;
        public int PopulationSize { get; set; } = 10;
        public double MutationRate { get; set; } = 0.1;

        public bool HolesUsing { get; set; } = false;
    }
}
