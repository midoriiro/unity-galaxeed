using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Math
{
    class NewtonRaphsonSolver
    {
        public float Initial { get; set; }
        public float Epsilon { get; set; }
        public Func<float, float> Function { get; set; }
        public Func<float, float> FunctionPrime { get; set; }
        public float MaxIterations { get; set; }

        public NewtonRaphsonSolver()
        {
            this.Epsilon = 0.01f;
            this.MaxIterations = 1000;
        }

        public NewtonRaphsonSolver(float initial, Func<float, float> f, Func<float, float> fp)
        {
            this.Initial = initial;
            this.Epsilon = 0.01f;
            this.Function = f;
            this.FunctionPrime = fp;
            this.MaxIterations = 1000;
        }

        public float Solve()
        {
            float x = this.Initial;
            float tolerance = this.Epsilon;
            float error = 10 * tolerance;
            int iterations = 0;

            while (iterations < this.MaxIterations)
            {
                float dx = -this.Function(x) / this.FunctionPrime(x);

                x = x + dx;

                error = Mathf.Abs(dx);

                if (error < tolerance)
                    break;

                iterations++;
            }

            return x;
        }
    }
}
