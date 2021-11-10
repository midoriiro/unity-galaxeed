using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Galaxeed.Math
{
    public class LegendrePolynomial
    {
        public float Epsilon { get; set; }
        public int MaxIterations { get; set; }

        private readonly int _n;
        private readonly List<float> _r;
        private readonly List<float> _w;

        private static readonly Dictionary<int, LegendrePolynomial> Tables = new Dictionary<int, LegendrePolynomial>();

        private LegendrePolynomial(int n)
        {
            this.Epsilon = 0.01f;
            this.MaxIterations = 1000;
            this._n = n;
            this._r = new List<float>();
            this._w = new List<float>();
        }

        public static LegendrePolynomial GetInstance(int n)
        {
            LegendrePolynomial legendrePolynomial;

            if(LegendrePolynomial.Tables.TryGetValue(n, out legendrePolynomial))
                return legendrePolynomial;

            legendrePolynomial = new LegendrePolynomial(n);
 
            LegendrePolynomial.Tables.Add(n, legendrePolynomial);

            return legendrePolynomial;
        }

        public float GetPolynomial(int n, float x, bool derivative = false)
        {
            if(!derivative)
            {
                if (n == 0)
                    return 1f;
                if (n == 1)
                    return x;

                return ((2f * n - 1f) * (x * this.GetPolynomial(n - 1, x)) - (n - 1) * this.GetPolynomial(n - 2, x)) / n;
            }
            else
            {
                if (n == 0)
                    return 0f;
                if (n == 1)
                    return 1f;

                return n / ( x * x - 1f ) * ( x * this.GetPolynomial(n, x) - this.GetPolynomial(n - 1, x) );
            }            
        }

        public float[] GetRoots()
        {
            if(this._r.Count > 0)
                return this._r.ToArray();

            for(int i = 1 ; i <= this._n; ++i)
            {
                float initial = Mathf.Cos(Mathf.PI * (i - 0.25f) / (this._n + 0.5f));

                NewtonRaphsonSolver solver = new NewtonRaphsonSolver()
                {
                    Epsilon = this.Epsilon,
                    MaxIterations = this.MaxIterations,
                    Function = (x) => this.GetPolynomial(this._n, x),
                    FunctionPrime = (x) => this.GetPolynomial(this._n, x, true),
                    Initial = initial
                };

                this._r.Add(solver.Solve());
            }

            return this._r.ToArray();
        }

        public float[] GetWeights()
        {
            if(this._w.Count > 0)
                return this._w.ToArray();

            for(int i = 0 ; i < this._r.Count ; ++i)
            {
                float x = this._r[i];
                float x1 = this.GetPolynomial(this._n, x, true);

                this._w.Add(2 / ( ( 1 - x * x ) * ( x1 * x1 ) ));
            }

            return this._w.ToArray();
        }

        public List<KeyValuePair<float, float>> GetResult()
        {
            List<KeyValuePair<float, float>> result = new List<KeyValuePair<float, float>>();

            float[] roots = this.GetRoots();
            float[] weigths = this.GetWeights();

            for (int i = 0; i < roots.Length; i++)
                result.Add(new KeyValuePair<float, float>(weigths[i], roots[i]));

            return result.OrderByDescending(e => e.Key).ToList();
        }

        public float GetIntegral(float a, float b, Func<float, float> f)
        {
            float c1 = ( b - a ) / 2;
            float c2 = ( a + b ) / 2;
            float sum = 0;

            var result = this.GetResult();

            for(int i = 0 ; i < this._n ; ++i)
            {
                float t = c1 * result[i].Value + c2;
                sum += result[i].Key * f(t);
            }

            return c1 * sum;
        }
    }

}

