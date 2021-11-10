using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Unity
{
	public class GridData : ScriptableObject
	{
		public enum DataType 
		{
			Regular,
			Axonometric,
			Hexametric
		}

		[Flags]
		public enum SnapType
		{
			Point = 1,
			Frame = 2,
			Line = 4,
			Nearest = 8
		}

		[SerializeField]
		private DataType _selectedDataType;
		public DataType SelectedDataType
		{
			get
			{
				return this._selectedDataType;
			}
			set
			{
				this._selectedDataType = value;

				this._strategy = this._availableStrategy[this._selectedDataType];
			}
		}

		[SerializeField]
		private SnapType _selectedSnapType;
		public SnapType SelectedSnapType
		{
			get { return this._selectedSnapType; }
			set { this._selectedSnapType = value; }
		}

		[SerializeField]
		private List<List<Vector2>> _data;
		public List<List<Vector2>> Data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		[SerializeField]
		private IGridDataStrategy _strategy;
		public IGridDataStrategy Strategy
		{
			get
			{
				return this._strategy;
			}
			set
			{
				this._strategy = value;
			}
		}

		[SerializeField]
		private Dictionary<DataType, IGridDataStrategy> _availableStrategy;

		[SerializeField]
		private List<List<Dictionary<string, Vector2>>> _frames;
		public List<List<Dictionary<string, Vector2>>> Frames
		{
			get
			{
				return _frames;
			}
			set
			{
				this._frames = value;
			}
		}

		[SerializeField]
		private Vector2 _center;
		public Vector2 Center
		{
			get
			{
				if (this._data.Count > 0)
					return (this._data.First().First() + this._data.Last().Last()) / 2;

				return this._center;
			}
			private set
			{
				this._center = value;
			}
		}

		[SerializeField]
		private Vector2Int _quantize;
		public Vector2Int Quantize
		{
			get
			{
				if (this.SelectedDataType == DataType.Hexametric)
				{
					int y = this._quantize.y;
					y -= 1;

					return new Vector2Int(this._quantize.x, y);
				}

				return this._quantize;
			}
			set
			{
				if (this.SelectedDataType == DataType.Hexametric)
				{
					value.y += 1;
				}

				this._quantize = value;

				if (this.QuantizeChanged != null)
					this.QuantizeChanged(this, null);
			}
		}

		[SerializeField]
		private Vector2 _tileSize;
		public Vector2 TileSize
		{
			get
			{
				return this._tileSize;
			}
			set
			{
				this._tileSize = value;

				if (this.TileSizeChanged != null)
					this.TileSizeChanged(this, null);
			}
		}

		[SerializeField]
		private Vector2 _tileSkew;
		public Vector2 TileSkew
		{
			get
			{
				return this._tileSkew;
			}
			set
			{
				this._tileSkew = value;
			}
		}

		[SerializeField]
		private Vector2Int _tileSkewSkip;
		public Vector2Int TileSkewSkip
		{
			get
			{
				return this._tileSkewSkip;
			}
			set
			{
				this._tileSkewSkip = value;
			}
		}

		[SerializeField]
		private Vector2 _mapSize;
		public Vector2 MapSize
		{
			get
			{
				return this._mapSize;
			}
			set
			{
				this._mapSize = value;

				if (this.MapSizeChanged != null)
					this.MapSizeChanged(this, null);
			}
		}

		[SerializeField]
		private float _snapResolution;
		public float SnapResolution
		{
			get
			{
				return this._strategy.Resolution / 2;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public event EventHandler DataChanged;
		private event EventHandler QuantizeChanged;
		private event EventHandler TileSizeChanged;
		private event EventHandler MapSizeChanged;

		private void OnQuantizeChanged(object sender, EventArgs args)
		{
			this._mapSize.x = this._tileSize.x * this._quantize.x;
			this._mapSize.y = this._tileSize.y * this._quantize.y;
		}

		private void OnTileSizeChanged(object sender, EventArgs args)
		{
			this._mapSize.x = this._tileSize.x * this._quantize.x;
			this._mapSize.y = this._tileSize.y * this._quantize.y;
		}

		private void OnMapSizeChanged(object sender, EventArgs args)
		{
			this._tileSize.x = this._mapSize.x / this._quantize.x;
			this._tileSize.y = this._mapSize.y / this._quantize.y;
		}

		private void OnEnable()
		{
			if (this._data == null)
				this._data = new List<List<Vector2>>();

			if (this._frames == null)
				this._frames = new List<List<Dictionary<string, Vector2>>>();

			if(this._availableStrategy == null)
			{ 
				var regular = ScriptableObject.CreateInstance<GridDataRegular>();
				regular.Initialize(this);

				var axonometric = ScriptableObject.CreateInstance<GridDataAxonometric>();
				axonometric.Initialize(this);

				var hexametric = ScriptableObject.CreateInstance<GridDataHexametric>();
				hexametric.Initialize(this);

				this._availableStrategy = new Dictionary<DataType, IGridDataStrategy>()
				{
					{ DataType.Regular, regular },
					{ DataType.Axonometric, axonometric },
					{ DataType.Hexametric, hexametric }
				};

				this.SelectedDataType = this.SelectedDataType;
			}

			if(this.QuantizeChanged == null)
				this.QuantizeChanged += this.OnQuantizeChanged;
			
			if(this.TileSizeChanged == null)
				this.TileSizeChanged += this.OnTileSizeChanged;
			
			if(this.MapSizeChanged == null)
				this.MapSizeChanged += this.OnMapSizeChanged;
		}

		public void Initialyze()
		{
			this._center = new Vector2(0.5f, 0.5f);
			this._quantize = new Vector2Int(1, 1);
			this._tileSize = new Vector2(1f, 1f);
			this._tileSkew = new Vector2(0, 0);
			this._tileSkewSkip = new Vector2Int(0, 0);
			this._mapSize = new Vector2(1f, 1f);
		}

		public void Clear()
		{
			this._data.Clear();
			this._frames.Clear();
		}

		public void Generate(Vector2 position)
		{
			if (this.Center != position)
				this.Clear();

			if(this._data.Count > 0)
				return;

			float resolutionX = 1f / this._quantize.x;
			float resolutionY = 1f / this._quantize.y;

			for (int y = 0; y <= this._quantize.y; y++)
			{
				var row = new List<Vector2>();

				for (int x = 0; x <= this._quantize.x; x++)
				{
					Vector2 point = new Vector2(x * resolutionX, y * resolutionY);

					point.x *= this.MapSize.x;
					point.y *= this.MapSize.y;

					if (this.TileSkewSkip.x > 0 && y % this._tileSkewSkip.x != 0)
						point.x += this._tileSkew.x;
					else if (this.TileSkewSkip.x == 0)
						point.x += this._tileSkew.x * y;

					if (this.TileSkewSkip.y > 0 && x % this._tileSkewSkip.y != 0)
						point.y += this._tileSkew.y;
					else if (this.TileSkewSkip.y == 0)
						point.y += this._tileSkew.y * x;

					row.Add(point);
				}

				this._data.Add(row);
			}

			this.Move(position);

			for (int y = 0; y < this._data.Count; y++)
			{
				List<Dictionary<string, Vector2>> row = new List<Dictionary<string, Vector2>>(); 

				for (int x = 0; x < this._data[y].Count; x++)
				{
					row.Add(this._strategy.GetFrameAt(x, y));
				}

				this._frames.Add(row);
			}
		}

		public void Move(Vector2 target)
		{
			Vector2 position = this._center;
			Vector2 direction = (target - position).normalized;
			float distance = Vector2.Distance(position, target);

			for (int i = 0; i < this._data.Count; i++)
			{
				for (int j = 0; j < this._data[i].Count; j++)
				{
					Vector2 point = this._data[i][j];
					this._data[i][j] = point + direction * distance;
				}
			}

			this._center = target;
		}

		public Vector2 SnapToNearestData(Vector2 position, List<Vector2> data)
		{
			float min = Mathf.Infinity;
			Vector2 nearest = Vector2.zero;

			for (int i = 0; i < data.Count; i++)
			{
				Vector2 point = data[i];
				float distance = Vector2.Distance(position, point);

				if (distance < min)
				{
					min = distance;
					nearest = point;
				}
			}

			return nearest;
		}

		public Vector2 SnapToNearest(Vector2 position)
		{
			List<Vector2> data = new List<Vector2>();
			data.Add(this.SnapToNearestPoint(position));
			data.Add(this.SnapToNearestFrame(position));
			data.Add(this.SnapToNearestLine(position));

			return this.SnapToNearestData(position, data);
		}

		public Vector2 SnapToNearestPoint(Vector2 position)
		{
			return this.SnapToNearestData(position, this._strategy.GetFlattenedPoints());
		}

		public Vector2 SnapToNearestFrame(Vector2 position)
		{
			return this.SnapToNearestData(position, this._strategy.GetFlattenedFramesCenter());
		}

		public Vector2 SnapToNearestLine(Vector2 position, bool symmetrical = true)
		{
			List<Vector2> data;

			if (symmetrical)
			{
				data = this._strategy.GetLinesCenter();
			}
			else
			{
				data = this._strategy.GetLinesCenter();
			}

			return this.SnapToNearestData(position, data);
		}

		public Vector2? SnapToData(Vector2 position, List<Vector2> data, bool fallback)
		{
			for (int i = 0; i < data.Count; i++)
			{
				Vector2 point = data[i];
				float distance = Vector2.Distance(position, point);

				if (distance < this.SnapResolution)
					return point;
			}

			if (fallback)
				return this.SnapToNearestData(position, data);

			return null;
		}

		public Vector2? SnapTo(Vector2 position, bool symmetrical = true)
		{
			List<Vector2?> data = new List<Vector2?>();
			bool fallback;

			if ((this.SelectedSnapType & SnapType.Nearest) != 0)
				fallback = true;
			else
				fallback = false;

			if ((this.SelectedSnapType & SnapType.Point) != 0)
			{
				var point = this.SnapToPoint(position, fallback);

				data.Add(point);
			}

			if ((this.SelectedSnapType & SnapType.Frame) != 0)
			{
				var point = this.SnapToFrame(position, fallback);

				data.Add(point);
			}

			if ((this.SelectedSnapType & SnapType.Line) != 0)
			{
				var point = this.SnapToLine(position, symmetrical, fallback);

				data.Add(point);
			}

			List<Vector2> concreteData = data
				.FindAll(e => e != null)
				.Select(e => (Vector2)e)
				.ToList();

			if (concreteData.Count == 0)
				return null;

			return concreteData.First();
		}

		public Vector2? SnapToPoint(Vector2 position, bool fallback = true)
		{
			return this.SnapToData(position, this._strategy.GetFlattenedPoints(), fallback);
		}

		public Vector2? SnapToFrame(Vector2 position, bool fallback = true)
		{
			return this.SnapToData(position, this._strategy.GetFlattenedFramesCenter(), fallback);
		}

		public Vector2? SnapToLine(Vector2 position, bool symmetrical = true, bool fallback = true)
		{
			List<Vector2> data;

			if(symmetrical)
			{
				data = this._strategy.GetLinesCenter();
			}
			else
			{
				data = this._strategy.GetLinesCenter();
			}

			return this.SnapToData(position, data, fallback);
		}
	}
}
