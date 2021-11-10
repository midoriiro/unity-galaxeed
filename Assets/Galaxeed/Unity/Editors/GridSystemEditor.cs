using Galaxeed.Unity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Galaxeed.Unity.Editors
{
	[CustomEditor(typeof(GridSystem))]
	[CanEditMultipleObjects]
	class GridSystemEditor : Editor
	{
		private SerializedProperty _grid;
		private SerializedProperty _display;

		private bool _quantizeBase2;

		private bool _quantizeFoldout;
		private bool _tileSizeFoldout;
		private bool _tileSkewFoldout;
		private bool _mapSizeFoldout;
		private bool _informationsFoldout;

		private void OnEnable()
		{
			this._grid = this.serializedObject.FindProperty("Grid");
			this._display = this.serializedObject.FindProperty("Display");

			this._quantizeBase2 = true;

			this._quantizeFoldout = true;
			this._tileSizeFoldout = true;
			this._tileSkewFoldout = true;
			this._mapSizeFoldout = true;
			this._informationsFoldout = true;
		}

		private void OnSceneGUI()
		{
			var grid = this._grid.objectReferenceValue as GridData;

			var camera = SceneView.currentDrawingSceneView.camera;
			var position = Event.current.mousePosition;
			position.y = camera.pixelHeight - position.y;

			var mouse = camera.ScreenToWorldPoint(position);

			var snap = grid.SnapTo(mouse);

			if(snap != null)
			{
				position = (Vector2)snap;

				Handles.color = Color.cyan;
				Handles.DrawSolidDisc((Vector2)snap, Vector3.forward, grid.SnapResolution / 2);
			}

			HandleUtility.Repaint();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var grid = this._grid.objectReferenceValue as GridData;
			var display = this._display.objectReferenceValue as GridDisplay;

			var selectedDebugType = display.SelectedDebugType;
			var selectedDataType = grid.SelectedDataType;
			var selectedSnapType = grid.SelectedSnapType;
			var gridQuantize = grid.Quantize;
			var tileSize = grid.TileSize;
			var tileSkew = grid.TileSkew;
			var tileSkewSkip = grid.TileSkewSkip;
			var mapSize = grid.MapSize;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Debug", GUILayout.MaxWidth(50));
			selectedDebugType = (GridDisplay.DebugType)EditorGUILayout.EnumMaskField(selectedDebugType);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Type", GUILayout.MaxWidth(50));
			selectedDataType = (GridData.DataType)EditorGUILayout.EnumPopup(selectedDataType);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Snap", GUILayout.MaxWidth(50));
			selectedSnapType = (GridData.SnapType)EditorGUILayout.EnumMaskField(selectedSnapType);
			EditorGUILayout.EndHorizontal();

			if ( this._quantizeFoldout = EditorGUILayout.Foldout(this._quantizeFoldout, "Quantize"))
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("1/1"))
					gridQuantize.x = 1;

				if (GUILayout.Button("1/2"))
					gridQuantize.x = 2;

				if (GUILayout.Button("1/4"))
					gridQuantize.x = 8;

				if (GUILayout.Button("1/8"))
					gridQuantize.x = 32;

				if (GUILayout.Button("1/16"))
					gridQuantize.x = 128;

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("x", GUILayout.MaxWidth(10));
				gridQuantize.x = EditorGUILayout.IntSlider(gridQuantize.x, 1, 128);
				
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("1/1"))
					gridQuantize.y = 1;

				if (GUILayout.Button("1/2"))
					gridQuantize.y = 2;

				if (GUILayout.Button("1/4"))
					gridQuantize.y = 8;

				if (GUILayout.Button("1/8"))
					gridQuantize.y = 32;

				if (GUILayout.Button("1/16"))
					gridQuantize.y = 128;

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("y", GUILayout.MaxWidth(10));
				gridQuantize.y = EditorGUILayout.IntSlider(gridQuantize.y, 1, 128);

				EditorGUILayout.EndHorizontal();
			}

			if (this._tileSizeFoldout = EditorGUILayout.Foldout(this._tileSizeFoldout, "Tile Size"))
			{
				// isometric = 1.73205 : 1
				// dimetric = 2 : 1
				// trimetric = 2 : 1 : 0.1374

				EditorGUI.BeginChangeCheck();

				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Square"))
				{
					tileSize.x = 1f;
					tileSize.y = 1;
					tileSkew.x = 0;
					tileSkew.y = 0;
				}

				if (GUILayout.Button("Milatary"))
				{
					tileSize.x = 1f;
					tileSize.y = 1;
					tileSkew.x = 1;
					tileSkew.y = -1;
				}

				if (GUILayout.Button("Cavalier"))
				{
					tileSize.x = 1f;
					tileSize.y = 1;
					tileSkew.x = 1;
					tileSkew.y = 0;
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Isometric"))
				{
					tileSize.x = 1.73205f;
					tileSize.y = 1;
					tileSkew.x = 0;
					tileSkew.y = 0;
				}

				if (GUILayout.Button("Dimetric"))
				{
					tileSize.x = 2;
					tileSize.y = 1;
					tileSkew.x = 0;
					tileSkew.y = 0;
				}

				if (GUILayout.Button("Trimetric"))
				{
					tileSize.x = 2;
					tileSize.y = 1;
					tileSkew.x = 0.2748f;
					tileSkew.y = 0;
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("x", GUILayout.MaxWidth(10));
				tileSize.x = EditorGUILayout.Slider(tileSize.x, 1, 400);

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("y", GUILayout.MaxWidth(10));
				tileSize.y = EditorGUILayout.Slider(tileSize.y, 1, 400);

				EditorGUILayout.EndHorizontal();

				if (EditorGUI.EndChangeCheck())
				{
					grid.TileSize = tileSize;
					grid.TileSkew = tileSkew;
				}
			}

			if(this._tileSkewFoldout = EditorGUILayout.Foldout(this._tileSkewFoldout, "Tile Skew"))
			{
				EditorGUI.BeginChangeCheck();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("row", GUILayout.MaxWidth(50));
				tileSkewSkip.x = EditorGUILayout.IntSlider(tileSkewSkip.x, 0, 400);

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("column", GUILayout.MaxWidth(50));
				tileSkewSkip.y = EditorGUILayout.IntSlider(tileSkewSkip.y, 0, 400);

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("x", GUILayout.MaxWidth(10));
				tileSkew.x = EditorGUILayout.Slider(tileSkew.x, -1, 1);

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("y", GUILayout.MaxWidth(10));
				tileSkew.y = EditorGUILayout.Slider(tileSkew.y, -1, 1);

				EditorGUILayout.EndHorizontal();

				if (EditorGUI.EndChangeCheck())
				{
					grid.TileSkewSkip = tileSkewSkip;
					grid.TileSkew = tileSkew;
				}
			}

			if (this._mapSizeFoldout = EditorGUILayout.Foldout(this._mapSizeFoldout, "Map Size"))
			{
				EditorGUI.BeginChangeCheck();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("x", GUILayout.MaxWidth(10));
				mapSize.x = EditorGUILayout.Slider(mapSize.x, 1, 400);

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("y", GUILayout.MaxWidth(10));
				mapSize.y = EditorGUILayout.Slider(mapSize.y, 1, 400);

				EditorGUILayout.EndHorizontal();

				if (EditorGUI.EndChangeCheck())
				{
					grid.MapSize = mapSize;
				}
			}

			if(this._informationsFoldout = EditorGUILayout.Foldout(this._informationsFoldout, "Informations"))
			{
				var frame = grid.Strategy.GetFrameAt(0, 0);

				float angleXZ = 0;
				float angleXY = 0;
				float angleYZ = 0;

				if (frame != null)
				{
					var top = frame["topCenter"];
					var bottom = frame["bottomCenter"];
					var left = frame["leftCenter"];
					var right = frame["rightCenter"];
					var topLeft = frame["topLeft"];
					var topRight = frame["topRight"];
					var bottomLeft = frame["bottomLeft"];
					var bottomRight = frame["bottomRight"];

					angleXZ = Vector2.Angle(left - top, right - top);
					angleXY = Vector2.Angle(bottom - left, topLeft - left);
					angleYZ = Vector2.Angle(bottom - right, topRight - right);
				}

				EditorGUILayout.LabelField("Angle XZ : " + angleXZ + "°");
				EditorGUILayout.LabelField("Angle XY : " + angleXY + "°");
				EditorGUILayout.LabelField("Angle YZ : " + angleYZ + "°");
			}

			if (EditorGUI.EndChangeCheck())
			{
				if(grid.Quantize != gridQuantize)
					grid.Quantize = gridQuantize;

				display.SelectedDebugType = selectedDebugType;
				// grid.CameraTrackingEnabled = cameraTracking;
				grid.SelectedDataType = selectedDataType;
				grid.SelectedSnapType = selectedSnapType;

				grid.Clear();
			}

			HandleUtility.Repaint();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
