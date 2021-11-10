using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Galaxeed.Math.Geometries
{
	public class PolyBezier
	{
		public BezierCurveOptions Options { get; set; }
		readonly List<BezierCurve> _curves;
		BezierCurve _buffer;

		public PolyBezier(BezierCurveOptions options)
		{
			this.Options = options;
			this._curves = new List<BezierCurve>();
			this._buffer = new BezierCurve(this.Options);
		}

		public static BezierCurve CreateCurve(Vector3[] points, BezierCurveOptions options)
		{
			PolyBezier poly = new PolyBezier(options);
			poly.AddRange(points);

			return poly.GetCurves()[0];
		}

		public static PolyBezier operator *(Quaternion q, PolyBezier p)
		{
			for (int i = 0; i < p._curves.Count; i++)
			{
				p._curves[i] = q * p._curves[i];
			}

			return p;
		}

		public void Add(Vector3 point)
		{
			try
			{
				this._buffer.Add(point);
			}
			catch (IndexOutOfRangeException)
			{
				Vector3 last = this._buffer.GetPoints()[this._buffer.GetPoints().Length - 1];

				this._curves.Add(this._buffer);
				this._buffer = new BezierCurve(this.Options);
				this._buffer.Add(last);
				this._buffer.Add(point);
			}
		}

		public void Add(BezierCurve curve)
		{
			if (this.Options.Type != curve.Options.Type)
				throw new BezierCurveException(
					"Adding a " +
					curve.Options.Type.ToString().ToLower() +
					" curve while poly bezier only handle curves of type " +
					this.Options.Type.ToString().ToLower()
				);

			this.AddRange(curve.GetPoints());
		}

		public void AddRange(Vector3[] points)
		{
			foreach (Vector3 point in points)
				this.Add(point);
		}

		public void AddRange(BezierCurve[] curves)
		{
			foreach (var curve in curves)
				this.Add(curve);
		}

		public int Count()
		{
			return this._curves.Count();
		}

		public bool IsTridimensional()
		{
			return this._curves.All(e => e.IsTridimensional());
		}

		public void Clear()
		{
			this._curves.Clear();
			this.ClearBuffer();
		}

		public void ClearBuffer()
		{
			this._buffer = null;
		}

		public bool IsBufferEmpty()
		{
			return this._buffer.GetPoints().Length == 0;
		}

		public float GetLinearLength()
		{
			return this._curves.Sum(e => e.GetLinearLength());
		}

		public float GetLength()
		{
			return this._curves.Sum(e => e.GetLength());
		}

		public float GetApproximateLength()
		{
			return this._curves.Sum(e => e.GetApproximateLength());
		}

		public Bounds GetBoundingBox()
		{
			Bounds result = new Bounds();
			Vector3 min = Vector3.positiveInfinity;
			Vector3 max = Vector3.negativeInfinity;

			foreach (var curve in this._curves)
			{
				Bounds box = curve.GetBoundingBox();

				if (box.min.x < min.x)
					min.x = box.min.x;

				if (box.max.x > max.x)
					max.x = box.max.x;

				if (box.min.y < min.y)
					min.y = box.min.y;

				if (box.max.y > max.y)
					max.y = box.max.y;

				if (box.min.z < min.z)
					min.z = box.min.z;

				if (box.max.z > max.z)
					max.z = box.max.z;
			}

			result.min = min;
			result.max = max;

			return result;
		}

		public float GetBoundingCircle()
		{
			float result = 0f;

			foreach (var curve in this._curves)
			{
				float r = curve.GetBoundingCircle();

				if (r > result)
					result = r;
			}

			return result;
		}

		public PolyLine GetPolylines()
		{
			PolyLine result = new PolyLine();

			foreach (var curve in this._curves)
				result.AddRange(curve.GetPoints());

			result.Distinct();

			return result;
		}

		public PolyLine GetOriginalPolylines()
		{
			PolyLine result = new PolyLine();

			foreach (var curve in this._curves)
			{
				Vector3[] lines = curve.GetOriginalPoints();

				if (lines != null)
					result.AddRange(lines);
			}

			result.Distinct();

			return result;
		}

		public Vector3 Compute(float t, int derivation = -1)
		{
			if (!(0f <= t && t <= this._curves.Count))
				throw new BezierCurveException("Argument t have to be between 0 and " + this._curves.Count);

			int i = (int)t;
			t = t - (float)System.Math.Truncate(t);

			BezierCurve curve;

			try
			{
				curve = this._curves[i];
			}
			catch(ArgumentOutOfRangeException)
			{
				curve = this._curves[i - 1];
				t = 1f;
			}

			return curve.Compute(t, derivation);
		}

		public PolyLine GetFlatened(int derivation = -1, float shift = 0f)
		{
			PolyLine result = new PolyLine();

			foreach (var curve in this._curves)
				result.Add(curve.GetFlatened(derivation, shift));

			result.Distinct();

			return result;
		}

		public PolyLine GetTangents(float shift = 0f)
		{
			PolyLine result = new PolyLine();
;
			foreach (var curve in this._curves)
				result.Add(curve.GetTangents(this.Options.Offset, shift));

			result.Distinct();

			return result;
		}

		public Vector3 GetTangent(float t, float distance = 1f)
		{
			Vector3 point = this.Compute(t);
			Vector3 tangent = this.Compute(t, this.Options.Degree - 1);

			return point + tangent.normalized * distance;
		}

		public PolyLine Get2DNormals()
		{
			PolyLine result = new PolyLine();

			foreach (var curve in this._curves)
				result.Add(curve.Get2DNormals(this.Options.Offset));

			//result.Distinct();

			return result;
		}

		public PolyLine[] Get3DNormals()
		{
			List<PolyLine> result = new List<PolyLine>()
			{
				new PolyLine(),
				new PolyLine()
			};

			foreach (var curve in this._curves)
			{
				PolyLine[] normals = curve.Get3DNormals(this.Options.Offset);

				result[0].Add(normals[0]);
				result[1].Add(normals[1]);
			}

			return result.ToArray();
		}

		public Vector3 Get2DNormal(float t, float distance = 1f)
		{
			Vector3 point = this.Compute(t);
			Vector3 tangent = this.GetTangent(t, distance);

			Vector3 normal = tangent - point;
			normal = Quaternion.Euler(0, 0, 90) * normal;
			normal = normal + point;

			return normal;
		}

		public Vector3[] Get3DNormal(float t, float distance = 1f)
		{
			Vector3 point = this.Compute(t);
			Vector3 tangent = this.GetTangent(t, distance);
			Vector3 shift = this.GetTangent(t + 0.001f, distance);

			Vector3[] result = new Vector3[2];

			Vector3 p = point;
			Vector3 r1 = tangent;
			Vector3 r2 = shift;

			Vector3 c = Vector3.Cross(r2, r1).normalized;

			Vector3 n = Quaternion.AngleAxis(90, p.normalized - r1) * r1;
			n = Quaternion.AngleAxis(90, tangent - p) * n;

			result[0] = p + c * distance;
			result[1] = p + n * distance;

			return result;
		}

		public PolyLine[] Get2DParallels(bool graduated = false, bool mirrored = false)
		{
			List<PolyLine> result = new List<PolyLine>();
			result.Insert(0, new PolyLine());

			if(mirrored)
				result.Insert(1, new PolyLine());

			int iLength = mirrored ? 2 : 1;

			for (int i = 0; i < iLength; i++)
			{
				PolyLine current = result[i];

				foreach (var curve in this._curves)
				{
					float distance = graduated ? 1f : this.Options.Offset;

					PolyLine[] parallels = curve.Get2DParallels(distance, mirrored);

					current.Add(i == 1 ? parallels[1] : parallels[0]);
				}

				current.Distinct();

				if (!graduated) continue;

				PolyLine flat = this.GetFlatened();
				flat.Distinct();

				for (int j = 0; j < current.Count(); j++)
				{
					float offset = MathHelper.Map(j, 0, current.Count() - 1, 0, this.Options.Offset);

					current[j] = flat[j] + (current[j] - flat[j]).normalized * offset;
				}
			}

			return result.ToArray();
		}

		public Vector3 Get2DParallel(float t, bool graduated = false, bool mirrored = false)
		{
			float distance = graduated ? 1f : this.Options.Offset;

			Vector3 point = this.Compute(t);
			Vector3 normal = this.Get2DNormal(t, distance);

			if(graduated)
			{
				float offset = MathHelper.Map(t, 0f, this._curves.Count, 0f, this.Options.Offset);

				normal = point + (normal - point).normalized * offset;
			}

			if(mirrored)
			{
				normal = normal * -1;
			}

			return normal;
		}

		public PolyLine[][] GetComponents(int derivation = -1)
		{
			List<PolyLine[]> result = new List<PolyLine[]>();

			foreach (var curve in this._curves)
				result.Add(curve.GetComponents(derivation));

			return result.ToArray();
		}

		public float GetIntervalFromDistance(float distance)
		{
			float length = this.GetLength();

			if (!(0f <= distance && distance <= length))
				throw new BezierCurveException("Distance have to be between 0 and " + length);

			return MathHelper.Map(distance, 0f, length, 0f, this._curves.Count);
		}

		public Vector3[][] GetExtremities()
		{
			List<List<Vector3>> result = new List<List<Vector3>>();

			for (int i = 0; i < this._curves.Count; i++)
			{
				var components = this._curves[i].GetComponentsExtremities(this.Options.Degree - 1);

				foreach(var component in components)
				{
					var current = new List<Vector3>();

					foreach(var extremity in component)
					{
						current.Add(this._curves[i].Compute(extremity.x));
					}

					result.Add(current);
				}
			}

			return result.Select(e => e.ToArray()).ToArray();
		}

		public float[] GetUncomputedExtremities(float tolerance = -1)
		{
			List<float> result = new List<float>();

			for (int i = 0; i < this._curves.Count; i++)
			{
				var extremities = this._curves[i].GetComponentsExtremities(this.Options.Degree - 1).Select(e => e.ToList()).ToList();

				List<Vector3> merge = new List<Vector3>();

				for (int j = 0; j < extremities.Count; j++)
				{
					var componentExtremities = extremities[j];

					merge.AddRange(componentExtremities.Select(e => e));
				}

				merge = merge.OrderBy(e => e.x).ToList();

				var times = merge.Select(e => e.x).ToList();
				times = times.Distinct().ToList();

				if(tolerance > 0f)
				{
					List<float> tolered = new List<float>();

					for (int j = 0; j < times.Count; j++)
					{
						if (j == 0) continue;

						float o = times[j - 1];
						float n = times[j];

						if (o == 0f || n == 1f) continue;

						float d = n - o;

						if (d < tolerance)
						{
							tolered.Add((o + n) / 2);
							j++;
						}
						else
						{
							tolered.Add(o);
						}
					}

					tolered.Add(0f);
					tolered.Add(1f);

					tolered = tolered.OrderBy(e => e).ToList();

					times = tolered;
				}

				result.AddRange(times.Select(e => e + i));
			}

			result = result.Distinct().ToList();

			return result.ToArray();
		}

		public BezierCurve[] GetCurves()
		{
			return this._curves.ToArray();
		}

		public bool IsContinuous(BezierCurveOptions.CurveContinuity continuity)
		{
			if (this._curves.Count < 2)
				throw new BezierCurveException("To test continuity of a poly bezier, it take at least two curves");

			for (int i = 0; i < this._curves.Count; ++i)
			{
				if (i == 0)
					continue;

				BezierCurve p = this._curves[i - 1];
				BezierCurve q = this._curves[i];

				if (!this.IsContinuous(p, q, continuity))
					return false;
			}

			return true;
		}

		public bool IsContinuous(BezierCurve p, BezierCurve q, BezierCurveOptions.CurveContinuity continuity)
		{
			Vector3[] pp = p.GetPoints();
			Vector3[] qp = q.GetPoints();

			if (continuity == BezierCurveOptions.CurveContinuity.C0)
			{
				Vector3 p3 = pp[pp.Length - 1];
				Vector3 q0 = qp[0];

				if (p3 != q0)
					return false;
			}
			else if (continuity == BezierCurveOptions.CurveContinuity.C1)
			{
				Vector3 p2 = pp[pp.Length - 2];
				Vector3 p3 = pp[pp.Length - 1];
				Vector3 q0 = qp[0];
				Vector3 q1 = qp[1];

				if (p2 - p3 != q0 - q1)
					return false;
			}
			else if (continuity == BezierCurveOptions.CurveContinuity.C2 && this.Options.Type != BezierCurveOptions.CurveType.Linear)
			{
				Vector3 p1 = pp[pp.Length - 3];
				Vector3 p2 = pp[pp.Length - 2];
				Vector3 p3 = pp[pp.Length - 1];
				Vector3 q0 = qp[0];
				Vector3 q1 = qp[1];
				Vector3 q2 = qp[2];

				if (p1 - 2 * p2 + p3 != q0 - 2 * q1 + q2)
					return false;
			}

			return true;
		}

		public void JoinAs(BezierCurveOptions.CurveContinuity continuity)
		{
			if (this._curves.Count < 2)
				throw new BezierCurveException("To join bezier curves it take at least two curves");

			for (int i = 0; i < this._curves.Count; ++i)
			{
				if (i == 0)
					continue;

				BezierCurve p = this._curves[i - 1];
				BezierCurve q = this._curves[i];

				this.JoinAs(p, q, continuity);
			}
		}

		public void JoinAs(BezierCurve p, BezierCurve q, BezierCurveOptions.CurveContinuity continuity)
		{
			Vector3[] pp = p.GetPoints();
			Vector3[] qp = q.GetPoints();

			if (continuity == BezierCurveOptions.CurveContinuity.None)
			{
				p.Reset();
				q.Reset();
			}
			else if (continuity == BezierCurveOptions.CurveContinuity.C0)
			{
				// p3 == q0

				Vector3 p3 = pp[pp.Length - 1];

				if (!this.IsContinuous(p, q, continuity))
				{
					q.Replace(p3, 0);
				}
			}
			else if (continuity == BezierCurveOptions.CurveContinuity.C1)
			{
				// p2 - p3 == q0 - q1

				Vector3 p2 = pp[pp.Length - 2];
				Vector3 p3 = pp[pp.Length - 1];

				if (!this.IsContinuous(p, q, continuity))
				{
					p2 = MathHelper.GetLinearVector3(2f, p2, p3);
					q.Replace(p2, 1);
				}
			}
			else if (continuity == BezierCurveOptions.CurveContinuity.C2 && this.Options.Type != BezierCurveOptions.CurveType.Linear)
			{
				// p1 - 2 * p2 + p3 == q0 - 2 * q1 + q2

				Vector3 p2 = pp[pp.Length - 2];
				Vector3 p3 = pp[pp.Length - 1];
				Vector3 q0 = qp[0];
				Vector3 q1 = qp[1];

				if (!this.IsContinuous(p, q, continuity))
				{
					float pl = (p2 - p3).magnitude;
					float ql = (q1 - q0).magnitude;

					float l = MathHelper.Map(ql, 0f, pl, 0f, 1f);

					p2 = MathHelper.GetLinearVector3(1f + l, p2, p3);
					q.Replace(p2, 1);
				}
			}
		}

		public void LoopAs(BezierCurveOptions.CurveContinuity continuity)
		{
			if (this._curves.Count < 2)
				throw new BezierCurveException("To loop a poly bezier it take at least two curves");

			if(continuity == BezierCurveOptions.CurveContinuity.None)
				throw new BezierCurveException("CurveContinuity.None is not a valid continuity to end loop");

			BezierCurve p = this._curves[this._curves.Count - 1];
			BezierCurve q = this._curves[0];

			this.LoopAs(p, q, continuity);
		}

		private void LoopAs(BezierCurve p, BezierCurve q, BezierCurveOptions.CurveContinuity continuity)
		{
			Vector3[] pp = p.GetPoints();
			Vector3[] qp = q.GetPoints();

			if (continuity == BezierCurveOptions.CurveContinuity.C0)
			{
				// p3 == q0

				Vector3 q0 = qp[0];

				p.Replace(q0, pp.Length - 1);
			}
			else if (continuity == BezierCurveOptions.CurveContinuity.C1)
			{
				// p2 - p3 == q0 - q1

				this.LoopAs(p, q, BezierCurveOptions.CurveContinuity.C0);

				Vector3 q0 = qp[0];
				Vector3 q1 = qp[1];

				q1 = MathHelper.GetLinearVector3(2f, q1, q0);
				p.Replace(q1, pp.Length - 2);
			}
			else if (continuity == BezierCurveOptions.CurveContinuity.C2 && this.Options.Type != BezierCurveOptions.CurveType.Linear)
			{
				// p1 - 2 * p2 + p3 == q0 - 2 * q1 + q2

				this.LoopAs(p, q, BezierCurveOptions.CurveContinuity.C0);

				Vector3 p2 = pp[pp.Length - 2];
				Vector3 p3 = pp[pp.Length - 1];
				Vector3 q0 = qp[0];
				Vector3 q1 = qp[1];

				float pl = (p2 - p3).magnitude;
				float ql = (q1 - q0).magnitude;

				float l = MathHelper.Map(ql, 0f, pl, 0f, 1f);

				q1 = MathHelper.GetLinearVector3(1f + l, q1, q0);
				p.Replace(q1, pp.Length - 2);
			}
		}
	}
}
