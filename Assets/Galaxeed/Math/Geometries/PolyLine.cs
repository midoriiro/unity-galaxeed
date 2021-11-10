using System.Collections.Generic;
using UnityEngine;

namespace Galaxeed.Math.Geometries
{
    public class PolyLine
    {
        private readonly List<Vector3> _points;

        public Vector3 this[int key]
        {
            get
            {
                return this._points[key];
            }
            set
            {
                this._points[key] = value;
            }
        }

        public PolyLine()
        {
            this._points  = new List<Vector3>();
        }

        public static PolyLine operator +(Vector3 d, PolyLine p)
        {
            for (int i = 0; i < p._points.Count; i++)
                p._points[i] = p._points[i] + d;

            return p;
        }

        public static PolyLine operator -(Vector3 d, PolyLine p)
        {
            for (int i = 0; i < p._points.Count; i++)
                p._points[i] = p._points[i] - d;

            return p;
        }

        public static Vector3 Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float x1 = p1.x;
            float x2 = p2.x;
            float x3 = p3.x;
            float x4 = p4.x;

            float y1 = p1.y;
            float y2 = p2.y;
            float y3 = p3.y;
            float y4 = p4.y;

            float a1 = y2 - y1;
            float b1 = x1 - x2;
            float c1 = a1 * x1 + b1 * y1;

            float a2 = y4 - y3;
            float b2 = x3 - x4;
            float c2 = a2 * x3 + b2 * y3;

            float delta = a1 * b2 - a2 * b1;

            return new Vector3((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }

        public Vector3 Intersect(Vector3 p1, Vector3 p2)
        {
            return Vector3.zero;
        }

        public void Add(Vector3 point)
        {
            this._points.Add(point);
        }

        public void Add(PolyLine poly)
        {
            this._points.AddRange(poly.GetPoints());
        }

        public void AddRange(Vector3[] points)
        {
            this._points.AddRange(points);
        }

        public Vector3[] GetPoints()
        {
            return this._points.ToArray();
        }

        public Line[] GetLines()
        {
            List<Line> result = new List<Line>();

            for (int i = 0; i < this._points.Count; i++)
            {
                if(i == 0) continue;

                Vector3 s = this._points[i - 1];
                Vector3 e = this._points[i];

                result.Add(new Line(s, e));
            }

            return result.ToArray();
        }

        public void Distinct()
        {
            List<int> duplicates = new List<int>();

            for (int i = 0; i < this._points.Count; i++)
            {
                if (i == 0) continue;

                var previous = this._points[i - 1];
                var current = this._points[i];

                if (previous == current)
                    duplicates.Add(i - 1);
            }

            for (int i = duplicates.Count - 1; i >= 0; i--)
            {
                this._points.RemoveAt(duplicates[i]);
            }
        }

        public float DistanceBetweenLines()
        {
            float length = 0f;

            for (int i = 0; i < this._points.Count; i++)
            {
                if (i == 0) continue;

                var previous = this._points[i - 1];
                var current = this._points[i];

                length += (previous - current).magnitude;
            }

            return length / this._points.Count;
        }

        public int Count()
        {
            return this._points.Count;
        }
    }
}
