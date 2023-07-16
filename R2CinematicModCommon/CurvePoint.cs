using Toe;

namespace R2CinematicModCommon
{
    public class CurvePoint
    {
        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public float Fov { get; set; }

        public CurvePoint(Vector3 position, Vector3 rotation, float fov)
        {
            Position = position;
            Rotation = rotation;
            Fov = fov;
        }
    }
}