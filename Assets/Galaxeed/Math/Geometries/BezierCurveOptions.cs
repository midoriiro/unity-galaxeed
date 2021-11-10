using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxeed.Math.Geometries
{
	public class BezierCurveOptions
	{
		public enum CurveType { Linear, Quadratic, Cubic, NthOrder };
		public CurveType Type;

		public enum CurveContinuity { None, C0, C1, C2 };
		public CurveContinuity Continuity;

		public int Order { get; set; }
		public int Degree { get; set; }

		public bool Optimized { get; set; }
		public bool Uniformized { get; set; }

		private float _resolution;
		public float Resolution
		{
			get { return this._resolution; }
			set
			{
				if (value <= 0)
					value = 0.01f;
				else if (value >= 1f)
					value = 1f;

				this._resolution = value;
			}
		}

		private int _segments;
		public int Segments
		{
			get { return this._segments; }
			set
			{
				if (value <= 1)
					value = 1;

				this._segments = value;
				this._resolution = 1f / this._segments;
			}
		}

		private float _offset;
		public float Offset
		{
			get { return this._offset; }
			set
			{
				if (value <= 0)
					value = 0.1f;

				this._offset = value;
			}
		}

		public BezierCurveOptions(int order)
		{
			if(order < 5)
				throw new BezierCurveException("Use BezierCurveOptions(BezierCurveType) constructor");

			this.Type = CurveType.NthOrder;
			this.Order = order;
			this.Degree = order - 1;

			this.Initialyze();

		}

		public BezierCurveOptions(CurveType type)
		{
			this.Type = type;

			if (this.Type == CurveType.Linear)
			{
				this.Degree = 1;
				this.Order = 2;
			}
			else if (this.Type == CurveType.Quadratic)
			{
				this.Degree = 2;
				this.Order = 3;
			}
			else if (this.Type == CurveType.Cubic)
			{
				this.Degree = 3;
				this.Order = 4;
			}
			else if (this.Type == CurveType.NthOrder)
			{
				throw new BezierCurveException("Use BezierCurveOptions(int) constructor");
			}

			this.Initialyze();
		}

		private void Initialyze()
		{
			this.Segments = 1;
			this.Offset = 0.01f;
			this.Optimized = true;
			this.Uniformized = false;
			this.Continuity = CurveContinuity.None;
		}
	}
}
