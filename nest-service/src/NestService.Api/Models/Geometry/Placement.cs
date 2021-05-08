
namespace NestService.Api.Models.Geometry
{
    public class Placement
    {
        public UniPath Path { get; set; }
        public double Rotation { get; set; }
        public UniPathPoint TranslationPoint { get; set; }

        public Placement(UniPath path, double rotation, UniPathPoint translationPoint)
        {
            Path = path;
            Rotation = rotation;
            TranslationPoint = translationPoint;
        }
    }
}
