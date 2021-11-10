using System.Collections;
using System.Collections.Generic;
using Galaxeed.Math.Geometries;
using UnityEngine;

using Helper;

namespace Helper
{
    public class MathHelper
    {
        public static Vector3 GetLinearVector3(float t, Vector3 p0, Vector3 p1)
        {
            BezierCurveOptions options = new BezierCurveOptions(BezierCurveOptions.CurveType.Linear);
            options.Optimized = false;

            BezierCurve curve = new BezierCurve(options);
            curve.Add(p0);
            curve.Add(p1);

            return curve.Compute(t);
        }

        public static Vector3 GetQuadraticVector3(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            BezierCurveOptions options = new BezierCurveOptions(BezierCurveOptions.CurveType.Quadratic);
            options.Optimized = false;

            BezierCurve curve = new BezierCurve(options);
            curve.Add(p0);
            curve.Add(p1);
            curve.Add(p2);

            return curve.Compute(t);
        }

        public static Vector3 GetCubicVector3(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            BezierCurveOptions options = new BezierCurveOptions(BezierCurveOptions.CurveType.Cubic);
            options.Optimized = false;

            BezierCurve curve = new BezierCurve(options);
            curve.Add(p0);
            curve.Add(p1);
            curve.Add(p2);
            curve.Add(p3);

            return curve.Compute(t);
        }

        public static unsafe float FastSqrt(float n)
        {
            long i;
            float x2, y;
            const float threehalfs = 1.5F;

            x2 = n * 0.5F;
            y = n;
            i = *(long*) &y;
            i = 0x5f3759df - (i >> 1); 
            y = *(float*)&i;
            y = y * (threehalfs - (x2 * y * y)); 

            return y;
        }

        public static int Factorial(int n)
        {
            int r = 1;

            for(int i = 2 ; i <= n ; ++i)
            {
                r = r * i;
            }

            return r;
        }

        public static int[] Binomial(int n)
        {
            int[] terms = new int[n+1];

            for(int i = 0 ; i < n+1 ; ++i)
            {
                terms[i] = MathHelper.Binomial(n, i);
            }

            return terms;
        }

        public static int Binomial(int n, int k)
        {
            return MathHelper.Factorial(n) / ( MathHelper.Factorial(k) * MathHelper.Factorial(n - k) );
        }

        public static float BernsteinPolynomial(float t, int n, int i)
        {
            float mt = Mathf.Pow(1f - t, n - i);
            float tt = Mathf.Pow(t, i);

            return MathHelper.Binomial(n, i) * mt * tt;
        }

        public static float GoldenRatio()
        {
            return (1 + Mathf.Sqrt(5)) / 2;
        }

        public static bool IsParallelLine(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float ax = p2.x - p1.x;
            float ay = p2.y - p1.y;

            float bx = p4.x - p3.x;
            float by = p4.y - p3.y;

            float c1 = ay / ax;
            float c2 = by / bx;

            return Equals(c1, c2);
        }

        public static Vector3 LineIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
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

            return new Vector3(( b2 * c1 - b1 * c2 ) / delta, ( a1 * c2 - a2 * c1 ) / delta);
        }

        public static float Map(float v, float s1, float e1, float s2, float e2)
        {
            return (v - s1) / (e1 - s1) * (e2 - s2) + s2;
        }
    }
}