using Galaxeed.Unity.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxeed.Unity
{
	[Serializable]
	public class GridDisplay : ScriptableObject
	{
		[Flags]
		public enum DebugType
		{
			Center = 1,
			Point = 2,
			Frame = 4,
			Line = 8,
			Bounds = 16
		}

		[SerializeField]
		private DebugType _selectedDebugType;
		public DebugType SelectedDebugType
		{
			get { return this._selectedDebugType; }
			set { this._selectedDebugType = value; }
		}

		[SerializeField]
		private GridData _grid;
		public GridData Grid 
		{
			get
			{
				return this._grid;
			}
			set
			{
				this._grid = value;
			}
		}

		public void OnDataChanged(object sender, EventArgs args)
		{
			this.Display();
		}

		public void Display()
		{
			if (this.Grid.Data.Count == 0) return;

			foreach (var line in this.Grid.Strategy.GetLines())
			{
				GizmoHelper.DrawLine(line[0], line[1], Color.yellow);
			}

			if ((this.SelectedDebugType & GridDisplay.DebugType.Frame) != 0)
			{
				foreach (var point in this.Grid.Strategy.GetFlattenedFramesCenter())
				{
					GizmoHelper.DrawCircle(point, this.Grid.SnapResolution, Color.green);
				}
			}

			if ((this.SelectedDebugType & GridDisplay.DebugType.Line) != 0)
			{
				foreach (var point in this.Grid.Strategy.GetLinesCenter())
				{
					GizmoHelper.DrawCircle(point, this.Grid.SnapResolution, Color.red);
				}
			}

			if ((this.SelectedDebugType & GridDisplay.DebugType.Point) != 0)
			{
				foreach (var point in this.Grid.Strategy.GetFlattenedPoints())
				{
					GizmoHelper.DrawCircle(point, this.Grid.SnapResolution, Color.cyan);
				}
			}

			if ((this.SelectedDebugType & GridDisplay.DebugType.Bounds) != 0)
				GizmoHelper.DrawCube(this.Grid.Strategy.Bounds, Color.magenta);

			if ((this.SelectedDebugType & GridDisplay.DebugType.Center) != 0)
				GizmoHelper.DrawCircle(this.Grid.Center, this.Grid.SnapResolution, Color.magenta);
		}
	}
}