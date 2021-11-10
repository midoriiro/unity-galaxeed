using System;

namespace Galaxeed.Math
{
    public class SimpsonSolver
    {
        public int Intervals { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public Func<float, float> Function { get; set; }

        public SimpsonSolver() {}

        public SimpsonSolver(Func<float, float> f, float a, float b, int n)
        {
            this.Function = f;
            this.Start = a;
            this.End = b;
            this.Intervals = n;
        }

        public float Solve()
        {
            if(this.Intervals % 2 != 0)
                throw new ArgumentException("this.Intervals must be even");

            float h = (this.End - this.Start) / this.Intervals;
            float s = this.Function(this.Start) + this.Function(this.End);

            for (int i = 1; i < this.Intervals; i += 2)
                s += 4 * this.Function(this.Start + i * h);

            for (int i = 2; i < this.Intervals - 1; i += 2)
                s += 2 * this.Function(this.Start + i * h);

            return s * h / 3f;
        }
    }
}