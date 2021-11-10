using UnityEngine;

namespace Galaxeed.Math.Geometries
{
    public class Line
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }

        public Line(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}