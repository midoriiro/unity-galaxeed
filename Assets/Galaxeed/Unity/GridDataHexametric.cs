using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Unity
{
	public class GridDataHexametric: ScriptableObject, IGridDataStrategy
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

				Vector2 tl= frame["topLeft"];
				Vector2 bl = frame["bottomLeft"];
				Vector2 c = frame["center"];
				Vector2 b = frame["bottomCenter"];

				float dw = Vector2.Distance(c, (tl + bl) / 2);
				float dx = Vector2.Distance(b, c);
				float dy = Vector2.Distance(b, bl);
				float dz = Vector2.Distance(bl, tl);

				float m = Mathf.Min(dw, dx, dy, dz);

				float max = Mathf.Max(this._grid.MapSize.x, this._grid.MapSize.y);
				float min = Mathf.Min(this._grid.MapSize.x, this._grid.MapSize.y);

				float f = min / max;

				this._resolution = m * f;

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

				var bounds = new Bounds();
				bounds.center = this._grid.Center;
				bounds.SetMinMax(
					new Vector3(xMin, yMin),
					new Vector3(xMax, yMax)
				);

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
			var frames = this.GetFrames();
			var result = new List<Vector2>();

			for (int y = 0; y < frames.Count; y++)
			{
				for (int x = 0; x < frames[y].Count; x++)
				{
					var frame = frames[y][x];

					if(y == 0)
					{
						result.Add(frame["bottomCenter"]);
						result.Add(frame["bottomLeft"]);
					}

					if(y % 2 == 1 && x == 0)
					{
						result.Add(frame["bottomLeft"]);
					}
						
					result.Add(frame["topLeft"]);
					result.Add(frame["topCenter"]);

					if (x == frames[y].Count - 1)
					{
						result.Add(frame["topRight"]);
						result.Add(frame["bottomRight"]);
					}
				}
			}

			return result;
		}

		private void MoveFrame(Dictionary<string, Vector2> frame, Vector2 target)
		{
			var keys = new List<string>(frame.Keys);

			foreach (var key in keys)
			{
				var point = frame[key];
				frame[key] = point + target;
			}
		}

		public Dictionary<string, Vector2> GetFrameAt(int x, int y)
		{
			Dictionary<string, Vector2> result = new Dictionary<string, Vector2>();

			try
			{
				Vector2 bottomBottomLeft = this.Data[y][x];
				Vector2 bottomBottomRight = this.Data[y][x + 1];
				Vector2 bottomTopRight = this.Data[y + 1][x + 1];
				Vector2 bottomTopLeft = this.Data[y + 1][x];
				Vector2 bottomBottomCenter = (bottomBottomRight + bottomBottomLeft) / 2;
				Vector2 bottomTopCenter = (bottomTopLeft + bottomTopRight) / 2;
				Vector2 bottomLeftCenter = (bottomBottomLeft + bottomTopLeft) / 2;
				Vector2 bottomRightCenter = (bottomTopRight + bottomBottomRight) / 2;

				Vector2 topBottomLeft = this.Data[y + 1][x];
				Vector2 topBottomRight = this.Data[y + 1][x + 1];
				Vector2 topTopRight = this.Data[y + 2][x + 1];
				Vector2 topTopLeft = this.Data[y + 2][x];
				Vector2 topBottomCenter = (topBottomRight + topBottomLeft) / 2;
				Vector2 topTopCenter = (topTopLeft + topTopRight) / 2;
				Vector2 topLeftCenter = (topBottomLeft + topTopLeft) / 2;
				Vector2 topRightCenter = (topTopRight + topBottomRight) / 2;

				Vector2 leftCenter = (topTopLeft + bottomBottomLeft) / 2;
				Vector2 rightCenter = (topTopRight + bottomBottomRight) / 2;
				Vector2 center = (bottomBottomCenter + topTopCenter) / 2;

				result.Add("center", center);
				result.Add("topLeft", topLeftCenter);
				result.Add("topRight", topRightCenter);
				result.Add("bottomRight", bottomRightCenter);
				result.Add("bottomLeft", bottomLeftCenter);
				result.Add("topCenter", topTopCenter);
				result.Add("rightCenter", rightCenter);
				result.Add("bottomCenter", bottomBottomCenter);
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
			var baseFrame = this.GetFrameAt(0, 0);

			if (baseFrame == null) return null;

			var leftRightCenter = (baseFrame["topLeft"] + baseFrame["topRight"]) / 2;
			var directionToRight = (baseFrame["topRight"] - leftRightCenter).normalized;
			var distanceToRight = Vector2.Distance(leftRightCenter, baseFrame["topRight"]);
			var directionToTop = (baseFrame["topCenter"] - leftRightCenter).normalized;
			var distanceToTop = Vector2.Distance(leftRightCenter, baseFrame["topCenter"]);

			int countLine = 1;

			var result = new List<List<Dictionary<string, Vector2>>>();		

			for (int y = 0; y < this.Data.Count; y++)
			{
				var row = new List<Dictionary<string, Vector2>>();

				for (int x = 0; x < this.Data[y].Count; x++)
				{
					var frame = this.GetFrameAt(x, y);

					if(y >= 1 && frame != null)
					{
						if (countLine % 2 == 0)
							this.MoveFrame(frame, directionToRight * distanceToRight);

						this.MoveFrame(frame, directionToTop * (distanceToTop * (countLine - 1)));
					}

					if (frame != null)
						row.Add(frame);

					if(countLine % 2 == 0 && x == this.Data[y].Count - 1 && row.Count > 0)
					{
						frame = new Dictionary<string, Vector2>(row[0]);

						this.MoveFrame(frame, -directionToRight * (distanceToRight * 2));

						row.Insert(0, frame);
					}
				}

				if(row.Count > 0)
					result.Add(row);

				countLine++;
			}

			var bottomLeft = result.First().First()["topCenter"];
			var bottomRight = result.First().Last()["topCenter"];
			var topLeft = result.Last().First()["bottomCenter"];
			var topRight = result.Last().Last()["bottomCenter"];

			var bottomCenter = (bottomLeft + bottomRight) / 2;
			var topCenter = (topLeft + topRight) / 2;

			var center = (bottomCenter + topCenter) / 2;
			var direction = (this._grid.Center - center).normalized;
			var distance = Vector2.Distance(center, this._grid.Center);

			result.ForEach(e => e.ForEach(p => this.MoveFrame(p, direction * distance)));

			return result;
		}

		public List<List<Vector2>> GetFramesCenter()
		{
			var frames = this.GetFrames();

			if (frames == null)
				return new List<List<Vector2>>();
				
			return frames.Select(e => 
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
						frame["bottomCenter"],
						frame["bottomLeft"]
					});

					result.Add(new List<Vector2>
					{
						frame["bottomLeft"],
						frame["topLeft"]
					});

					result.Add(new List<Vector2>
					{
						frame["topLeft"],
						frame["topCenter"]
					});

					result.Add(new List<Vector2>
					{
						frame["topCenter"],
						frame["topRight"]
					});

					result.Add(new List<Vector2>
					{
						frame["topRight"],
						frame["bottomRight"]
					});

					result.Add(new List<Vector2>
					{
						frame["bottomRight"],
						frame["bottomCenter"]
					});
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
