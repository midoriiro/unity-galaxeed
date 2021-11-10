using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Unity
{
	public class GridDataRegular : ScriptableObject, IGridDataStrategy
	{
		[SerializeField]
		private GridData _grid;
		public List<List<Vector2>> Data 
		{ 
			get 
			{
				return this._grid.Data;
			}
			set
			{
				this._grid.Data = value;
			}
		}

		[SerializeField]
		private float _resolution;
		public float Resolution 
		{ 
			get
			{
				var frame = this.GetFrameAt(0, 0);

				if (frame == null) return this._resolution;

				Vector2 b = frame["bottomLeft"];
				Vector2 r = frame["bottomRight"];
				Vector2 t = frame["topLeft"];

				float dx = Vector2.Distance(b, r);
				float dy = Vector2.Distance(b, t);

				float m = Mathf.Min(dx, dy);

				this._resolution = m / 2;

				return this._resolution;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Bounds Bounds
		{
			get
			{
				var points = this.GetFlattenedPoints();

				float xMax = points.Max(e => e.x);
				float yMax = points.Max(e => e.y);
				float xMin = points.Min(e => e.x);
				float yMin = points.Min(e => e.y);

				Vector2 min = new Vector3(xMin, yMin);
				Vector2 max = new Vector3(xMax, yMax);

				var bounds = new Bounds();
				bounds.SetMinMax(min, max);

				return bounds;
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public void Initialize(GridData grid)
		{
			this._grid = grid;
		}

		public List<Vector2> GetFlattenedPoints()
		{
			return this.Data.SelectMany(e => e).ToList();
		}

		public Dictionary<string, Vector2> GetFrameAt(int x, int y)
		{
			Dictionary<string, Vector2> result = new Dictionary<string, Vector2>();

			try
			{
				Vector2 bottomLeft = this.Data[y][x];
				Vector2 bottomRight = this.Data[y][x + 1];
				Vector2 topRight = this.Data[y + 1][x + 1];
				Vector2 topLeft = this.Data[y + 1][x];
				Vector2 topCenter = (topLeft + topRight) / 2;
				Vector2 rightCenter = (topRight + bottomRight) / 2;
				Vector2 bottomCenter = (bottomRight + bottomLeft) / 2;
				Vector2 leftCenter = (bottomLeft + topLeft) / 2;
				Vector2 center = (topLeft + bottomRight) / 2;

				result.Add("center", center);
				result.Add("topLeft", topLeft);
				result.Add("topRight", topRight);
				result.Add("bottomRight", bottomRight);
				result.Add("bottomLeft", bottomLeft);
				result.Add("topCenter", topCenter);
				result.Add("rightCenter", rightCenter);
				result.Add("bottomCenter", bottomCenter);
				result.Add("leftCenter", leftCenter);
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}

			return result;
		}

		public List<List<Dictionary<string, Vector2>>> GetFrames()
		{
			var result = new List<List<Dictionary<string, Vector2>>>();

			for (int y = 0; y < this.Data.Count; y++)
			{
				var row = new List<Dictionary<string, Vector2>>();

				for (int x = 0; x < this.Data[y].Count; x++)
				{
					var frame = this.GetFrameAt(x, y);

					if (frame != null)
						row.Add(frame);
				}

				if(row.Count > 0)
					result.Add(row);
			}

			return result;
		}

		public List<List<Vector2>> GetFramesCenter()
		{
			return this.GetFrames()
				.Select(e => 
					e.Select(p => p["center"])
					.ToList())
				.ToList();
		}

		public List<Vector2> GetFlattenedFramesCenter()
		{
			return this.GetFramesCenter()
				.SelectMany(e => e)
				.ToList();
		}

		public List<List<Vector2>> GetLines()
		{
			var frames = this.GetFrames();
			var result = new List<List<Vector2>>();

			for (int y = 0; y < frames.Count; y++)
			{
				for (int x = 0; x < frames[y].Count; x++)
				{
					var frame = frames[y][x];

					result.Add(new List<Vector2>
					{
						frame["bottomLeft"],
						frame["bottomRight"]
					});

					result.Add(new List<Vector2>
					{
						frame["bottomLeft"],
						frame["topLeft"]
					});

					if(y == frames.Count - 1)
					{
						result.Add(new List<Vector2>
						{
							frame["topLeft"],
							frame["topRight"]
						});
					}

					if (x == frames[y].Count - 1)
					{
						result.Add(new List<Vector2>
						{
							frame["bottomRight"],
							frame["topRight"]
						});
					}
				}
			}

			return result;
		}

		public List<Vector2> GetLinesCenter()
		{
			var lines = this.GetLines();
			var result = new List<Vector2>();

			for (int i = 0; i < lines.Count; i++)
			{
				var line = lines[i];

				result.Add((line[0] + line[1]) / 2);
			}

			return result;
		}

		public List<Vector2> GetFlattenedLines()
		{
			return this.GetLines()
				.SelectMany(e => e)
				.Distinct()
				.ToList();
		}
	}
}
