using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toe;

namespace R2CinematicMod
{
    public class BezierPoint
    {
        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public float Fov { get; set; }

        public BezierPoint(Vector3 position, Vector3 rotation, float fov) 
        { 
            Position = position;
            Rotation = rotation;
            Fov = fov;
        }
    }
}
