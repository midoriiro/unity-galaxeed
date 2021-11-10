using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helper;
using UnityEngine;

namespace Galaxeed.Math.Geometries
{
	public class BezierCurve
	{
		public BezierCurveOptions Options { get; set; }
		private List<Vector3> _points;
		private List<Vector3> _originalPoints;

		public BezierCurve(BezierCurveOptions options)
		{
			this.Options = options;
			this._points = new List<Vector3>(this.Options.Order);
			this._originalPoints = null;
		}

		public static BezierCurve operator *(Quaternion q, BezierCurve c)
		{
			for (int i = 0; i < c._points.Count; i++)
			{
				c._points[i] = q * c._points[i];
			}

			return c;
		}

		public void Add(Vector3 point)
		{
			if (this._points.Count < this.Options.Order)
				this._points.Add(point);
			else
				throw new IndexOutOfRangeException();
		}

		public void Replace(Vector3 point, int index)
		{
			if (this._originalPoints == null)
				this._originalPoints = new List<Vector3>(this._points);

			this._points[index] = point;
		}

		public void Reset()
		{
			if (this._originalPoints != null)
			{
				this._points = new List<Vector3>(this._originalPoints);
				this._originalPoints = null;
			}
		}

		public Vector3[] GetPoints()
		{
			return this._points.ToArray();
		}

		public Vector3[] GetDerivativePoints(int degree)
		{
			if (!(1 <= degree && degree < this.Options.Degree))
				throw new BezierCurveException("Derivative degree must be between 1 and " + (this.Options.Degree - 1));

			int n = this.Options.Degree;

			List<Vector3> result = new List<Vector3>();

			for (int i = 0; i < n; ++i)
				result.Add(n * (this._points[i + 1] - this._points[i]));

			return result.ToArray();
		}

		public Vector3[] GetOriginalPoints()
		{
			return this._originalPoints == null ? null : this._originalPoints.ToArray();
		}

		public bool IsTridimensional()
		{
			return this._points.Any(e => e.z != 0);
		}

		private Vector3 MoveTo(float t)
		{
			Vector3 r = Vector3.zero;

			for (int i = 0; i < this.Options.Order; ++i)
			{
				r += this._points[i] * MathHelper.BernsteinPolynomial(t, this.Options.Degree, i);
			}

			return r;
		}

		private Vector3 MoveByDerivation(float t, int degree)
		{
			Vector3[] dpoints = this.GetDerivativePoints(degree);
			int k = degree;

			Vector3 result = Vector3.zero;

			for (int i = 0; i < dpoints.Length; ++i)
				result += dpoints[i] * MathHelper.BernsteinPolynomial(t, k, i);

			return result;
		}

		private Vector3 MoveToRecursion(float t)
		{
			return this.MoveToRecursion(t, this._points.ToArray());
		}

		private Vector3 MoveToRecursion(float t, Vector3[] points)
		{
			if (points.Length == 1)
				return points[0];

			Vector3[] result = new Vector3[points.Length - 1];

			for (int i = 0; i < result.Length; ++i)
			{
				result[i] = MathHelper.GetLinearVector3(t, points[i], points[i + 1]);
			}

			return this.MoveToRecursion(t, result);
		}

		public Vector3 Compute(float t, int derivation = -1)
		{
			if(this._points.Count != this.Options.Order)
				throw new BezierCurveException(
					"There are not enough points, actual = " + 
					this._points.Count + 
					" expected = " + 
					this.Options.Order);

			if (1 <= derivation && derivation <= this.Options.Degree - 1)
				return this.MoveByDerivation(t, derivation);

			if (!this.Options.Optimized)
				return this.MoveTo(t);

			return this.MoveToRecursion(t);
		}

		public Vector3[][] Split(float t)
		{
			List<Vector3> left = new List<Vector3>();
			List<Vector3> right = new List<Vector3>();

			this.Split(t, this._points.ToArray(), left, right);

			List<List<Vector3>> result = new List<List<Vector3>>()
			{
				left,
				right
			};

			return result.Select(e => e.ToArray()).ToArray();
		}

		private void Split(float t, Vector3[] points, List<Vector3> left, List<Vector3> right)
		{
			if (points.Length == 1)
			{
				left.Add(points[0]);
				right.Add(points[0]);

				return;
			}

			Vector3[] result = new Vector3[points.Length - 1];

			for (int i = 0; i < result.Length; ++i)
			{
				if (i == 0)
					left.Add(points[i]);

				if (i == result.Length - 1)
					right.Add(points[i + 1]);

				result[i] = MathHelper.GetLinearVector3(t, points[i], points[i + 1]);
			}

			this.Split(t, result, left, right);
		}

		public float[] GetFlatenedIntervals()
		{
			List<float> result = new List<float>();

			if (!this.Options.Uniformized)
			{
				float t;

				for (t = 0f; t <= 1f; t = t + this.Options.Resolution)
					result.Add(t);

				if (!t.Equals(1f))
					result.Add(1f);
			}
			else
			{
				result.AddRange(this.GetIntervals(this.Options.Segments));
			}

			return result.ToArray();
		}

		public PolyLine GetFlatened(int derivation = -1, float shift = 0f)
		{
			float[] intervals = this.GetFlatenedIntervals();

			PolyLine result = new PolyLine();

			foreach (var interval in intervals)
				result.Add(this.Compute(interval + shift, derivation));

			return result;
		}

		public PolyLine GetTangents(float distance = 1f, float shift = 0f)
		{
			PolyLine flat = this.GetFlatened(shift: shift);
			PolyLine result = this.GetFlatened(this.Options.Degree - 1, shift);

			for (int i = 0; i < result.Count(); i++)
				result[i] = flat[i] + result[i].normalized * distance;

			return result;
		}

		public PolyLine Get2DNormals(float distance = 1f)
		{
			PolyLine flat = this.GetFlatened();
			PolyLine tangents = this.GetTangents(distance);

			PolyLine result = new PolyLine();

			for (int i = 0; i < tangents.Count(); ++i)
			{
				Vector3 d = tangents[i] - flat[i];
				d = Quaternion.Euler(0, 0, 90) * d;
				d = d + flat[i];

				result.Add(d);
			}

			return result;
		}

		public PolyLine[] Get3DNormals(float distance = 1f)
		{
			PolyLine flat = this.GetFlatened();
			PolyLine tangents = this.GetTangents(1f);
			PolyLine shifts = this.GetTangents(1f, 0.001f);

			List<PolyLine> result = new List<PolyLine>()
			{
				new PolyLine(),
				new PolyLine()
			};

			for (int i = 0; i < shifts.Count(); ++i)
			{
				Vector3 f = flat[i];
				Vector3 r1 = tangents[i].normalized;
				Vector3 r2 = shifts[i].normalized;

				Vector3 c = Vector3.Cross(r2, r1).normalized;

				Vector3 n = Quaternion.AngleAxis(90, f.normalized - r1) * r1;
				n = Quaternion.AngleAxis(90, tangents[i] - f) * n;

				c = f + c * distance;
				n = f + n * distance;

				result[0].Add(c);
				result[1].Add(n);
			}

			return result.ToArray();
		}

		public PolyLine[] Get2DParallels(float distance, bool mirrored = false)
		{
			PolyLine flat = this.GetFlatened();
			PolyLine normals = this.Get2DNormals(distance);
			List<PolyLine> result = new List<PolyLine>();

			int iLength = mirrored ? 2 : 1;

			for (int i = 0; i < iLength; i++)
			{
				PolyLine lines = new PolyLine();

				for (int j = 0; j < normals.Count(); j++)
				{
					Vector3 n = flat[j];

					if (i == 0)
						n += (normals[j] - flat[j]).normalized * distance;
					else
						n -= (normals[j] - flat[j]).normalized * distance;

					lines.Add(n);
				}

				result.Add(lines);
			}

			return result.ToArray();
		}

		public PolyLine[] GetComponents(int derivation = -1, float? t = null)
		{
			PolyLine xc = new PolyLine();
			PolyLine yc = new PolyLine();
			PolyLine zc = new PolyLine();
			PolyLine lc = new PolyLine();

			float total = this.GetLength();
			List<float> length = new List<float>();

			Action<float> f = dt =>
			{
				Vector3 v = this.Compute(dt, derivation).normalized;

				float x = v.x;
				float y = v.y;
				float z = v.z;

				xc.Add(new Vector3(dt, x));
				yc.Add(new Vector3(dt, y));
				zc.Add(new Vector3(dt, z));
			};

			Action<float> dF = dt =>
			{
				float l = MathHelper.Map(length.Sum(), 0f, total, 0f, 1f);

				lc.Add(new Vector3(dt, l));

				length.Add(this.GetLength(dt, dt + this.Options.Resolution));
			};

			List<PolyLine> result = new List<PolyLine>();

			if (t != null)
			{
				f((float)t);

				for (float i = 0f; i <= t; i = i + this.Options.Resolution)
					dF(i);
			}
			else
			{
				float i;

				for (i = 0f; i <= 1f; i = i + this.Options.Resolution)
				{
					f(i);
					dF(i);
				}

				if (!i.Equals(1f))
					f(1f);
			}

			Vector3 lastLength = lc[lc.Count() - 1];

			if (lastLength.y > 1f)
			{
				lastLength.y = 1f;
				lc[lc.Count() - 1] = lastLength;
			}
			else if (lastLength.y < 1f)
			{
				lc.Add(new Vector3(1f, 1f));
			}

			result.Add(xc);
			result.Add(yc);
			result.Add(zc);
			result.Add(lc);

			return result.ToArray();
		}

		public Vector3[][] GetComponentsExtremities(int derivation = -1)
		{
			float[] extremities = this.GetExtremities();

			List<List<Vector3>> result = new List<List<Vector3>>
			{
				new List<Vector3>(),
				new List<Vector3>(),
				new List<Vector3>()
			};

			foreach (var extremity in extremities)
			{
				PolyLine[] components = this.GetComponents(derivation, extremity);
				Array.Resize(ref components, components.Length - 1);

				for (int i = 0; i < components.Length; i++)
				{
					PolyLine polyline = components[i];

					result[i].AddRange(polyline.GetPoints());
				}
			}

			if (result[2].All(e => e.y == 0f))
				result.RemoveAt(result.Count - 1);

			result = result.Select(e => e.FindAll(p => 0f <= p.x && p.x <= 1f)).ToList();

			Func<List<Vector3>, List<Vector3>> f = (v) => 
			{
				List<Vector3> left = v.FindAll(e => e.x < 0.5f);
				List<Vector3> right = v.FindAll(e => e.x >= 0.5f);

				List<Vector3> r = new List<Vector3>();

				r.Add(v.Find(e => e.x == 0f));
				r.Add(v.Find(e => e.y == left.Min(p => p.y)));
				r.Add(v.Find(e => e.y == left.Max(p => p.y)));
				r.Add(v.Find(e => e.y == right.Min(p => p.y)));
				r.Add(v.Find(e => e.y == right.Max(p => p.y)));
				r.Add(v.Find(e => e.x == 1f));

				r.OrderBy(e => e.x);

				return r;
			};
				
			var filtered = result
				.Select(e => f(e))
				.Select(e => e.Distinct().ToList())
				.ToList();

			return filtered.Select(e => e.ToArray()).ToArray();
		}

		public float[][] GetLinearComponents(int derivative = -1)
		{
			Vector3[] points;

			if (derivative != -1)
				points = this.GetDerivativePoints(derivative);
			else
				points = this._points.ToArray();

			List<float> xc = new List<float>();
			List<float> yc = new List<float>();
			List<float> zc = new List<float>();

			List<float[]> result = new List<float[]>();

			for (int i = 0; i < points.Length; i++)
			{
				Vector3 p = points[i];

				xc.Add(p.x);
				yc.Add(p.y);
				zc.Add(p.z);
			}

			result.Add(xc.ToArray());
			result.Add(yc.ToArray());
			result.Add(zc.ToArray());

			return result.ToArray();
		}

		public float[] GetIntervals(int segments)
		{
			float length = this.GetLength();
			float segmentLength = length / segments;
			float distance = 0f + segmentLength;

			List<float> result = new List<float> {0f};

			for (int i = 1; i <= segments; i++)
			{
				result.Add(this.GetIntervalFromDistance(distance, length));

				distance += segmentLength;
			}

			return result.ToArray();
		}

		public float GetIntervalFromDistance(float distance, float length)
		{
			NewtonRaphsonSolver solver = new NewtonRaphsonSolver
			{
				Epsilon = 0.000001f,
				Function = (t) => this.GetLength(0f, t) - distance,
				FunctionPrime = (t) => this.Compute(t, this.Options.Degree - 1).magnitude,
				Initial = distance / length
			};
			
			return solver.Solve();
		}

		public float[] GetExtremities()
		{
			Func<Vector3, float> f = (v) => v.y * MathHelper.FastSqrt(v.sqrMagnitude);
			Func<Vector3, float> df = (v) => 1f / MathHelper.FastSqrt(v.sqrMagnitude);

			List<float> result = new List<float>();

			for (float i = 0f; i <= 1f; i+= this.Options.Resolution)
			{
				NewtonRaphsonSolver solver = new NewtonRaphsonSolver()
				{
					Initial = i,
					Function = (t) => f(this.Compute(t)),
					FunctionPrime = (t) => df(this.Compute(t, this.Options.Degree - 1))
				};

				result.Add(solver.Solve());
			}

			result.AddRange(new []{0f , 1f});

			var filtered = result
				.FindAll(e => 0f <= e && e <= 1f)
				.Distinct()
				.OrderBy(e => e);

			return filtered.ToArray();
		}

		public float GetLinearLength()
		{
			float result = 0f;

			for (int i = 0; i < this._points.Count; i++)
			{
				if(i == 0) continue;

				Vector3 o = this._points[i - 1];
				Vector3 n = this._points[i];

				result += (n - o).magnitude;
			}

			return result;
		}

		public float GetLength(float start = 0f, float end = 1f)
		{
			var solver = new SimpsonSolver();
			solver.Intervals = 24;
			solver.Start = start;
			solver.End = end;
			solver.Function = (t) => this.Compute(t, this.Options.Degree - 1).magnitude;

			return solver.Solve();
		}

		public float GetApproximateLength()
		{
			float length = 0f;
			PolyLine flat = this.GetFlatened();

			for (int i = 0; i < flat.Count(); ++i)
			{
				if (i <= 0)
					continue;

				Vector3 o = flat[i - 1];
				Vector3 n = flat[i];

				length += (n - o).magnitude;
			}

			return length;
		}

		public Bounds GetBoundingBox()
		{
			Bounds result = new Bounds();
			Vector3 min = Vector3.positiveInfinity;
			Vector3 max = Vector3.negativeInfinity;

			List<float> extremities = new List<float>();

			foreach (var component in this.GetComponentsExtremities(this.Options.Degree - 1))
				foreach (var v in component)
					extremities.Add(v.x);

			foreach (var extremity in extremities)
			{
				Vector3 point = this.Compute(extremity);

				if (point.x < min.x)
					min.x = point.x;

				if (point.x > max.x)
					max.x = point.x;

				if (point.y < min.y)
					min.y = point.y;

				if (point.y > max.y)
					max.y = point.y;

				if (point.z < min.z)
					min.z = point.z;

				if (point.z > max.z)
					max.z = point.z;
			}

			result.min = min;
			result.max = max;

			return result;
		}

		public float GetBoundingCircle()
		{
			float radius = 0f;
			PolyLine points = this.GetFlatened();

			foreach (Vector3 point in points.GetPoints())
			{
				float d = Vector3.Distance(point, points[0]);

				if (d > radius)
					radius = d;
			}

			return radius;
		}
	}
}
