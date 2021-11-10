using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Math
{
    class BinarySearchSolver
    {
        public float Start { get; set; }
        public float End { get; set; }
        public float Epsilon { get; set; }
        public int MaxIterations { get; set; }
        public Func<float, float> Function { get; set; }

        public BinarySearchSolver()
        {
            this.Epsilon = 0.01f;
            this.MaxIterations = 1000;
        }

        public BinarySearchSolver(float a, float b, Func<float, float> f)
        {
            this.Start = a;
            this.End = b;
            this.Function = f;
            this.Epsilon = 0.01f;
            this.MaxIterations = 1000;
        }

        public float Solve()
        {
            float a = this.Start;
            float b = this.End;
            float c = 0f;

            float tolerance = this.Epsilon;
            float error = 10 * tolerance;
            int iterations = 0;

            while (iterations < this.MaxIterations)
            {
                c = (a + b) / 2;

                float r = this.Function(c);

                error = Mathf.Abs(b - a) / 2;

                if (r.Equals(0f) || error < tolerance)
                    break;

                if (Mathf.Sign(r).Equals(Mathf.Sign(this.Function(a))))
                    a = c;
                else
                    b = c;

                iterations++;
            }

            return c;
        }
    }
}
