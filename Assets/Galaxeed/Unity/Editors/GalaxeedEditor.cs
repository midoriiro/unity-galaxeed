using Galaxeed.Generators;
using Galaxeed.Options;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Galaxeed.Unity.Editors
{
	[CustomEditor(typeof(Galaxeed))]
	[CanEditMultipleObjects]
	public class GalaxeedEditor : Editor
	{
		private SerializedProperty _generator;
		private SerializedProperty _seedLoop;
		private SerializedProperty _gizmos;

		void OnEnable()
		{
			this._generator = this.serializedObject.FindProperty("Generator");
			this._seedLoop = this.serializedObject.FindProperty("SeedLoop");
			this._gizmos = serializedObject.FindProperty("Gizmos");
		}

		public override bool RequiresConstantRepaint()
		{
			return false;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var generator = this._generator.objectReferenceValue as GalaxyGenerator;
			var options = generator.SeedOptions;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Gizmos", GUILayout.MaxWidth(75f));
			this._gizmos.intValue = (int)(Galaxeed.DrawGizmos)EditorGUILayout.EnumMaskField(
				(Galaxeed.DrawGizmos)this._gizmos.intValue);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			this._seedLoop.boolValue = EditorGUILayout.ToggleLeft(
				"Loop", 
				this._seedLoop.boolValue, 
				GUILayout.MaxWidth(50f));

			options.IsDirty = EditorGUILayout.ToggleLeft(
				"Dirty", 
				options.IsDirty, 
				GUILayout.MaxWidth(50f));
			EditorGUILayout.EndHorizontal();

			options.Seed.Edit("Seed", GUILayout.MaxWidth(75f));

			EditorGUILayout.Separator();

			Debug.Log(options.Items.Count);

			//this._options["Resolution"].Edit();
			//this._options["Density"].Edit();
			options.ItemAtKey<IEditable>("Scale").Edit("Scale", GUILayout.MaxWidth(75));
			options.ItemAtKey<IEditable>("Branches").Edit("Branches", GUILayout.MaxWidth(75));
			//options["Iterations"].Edit("Iterations", GUILayout.MaxWidth(75));
			//options["Systems"].Edit("Systems", GUILayout.MaxWidth(75));

			EditorGUILayout.Separator();
	
			options.ItemAtKey<IEditable>("Type").Edit(
				"Type", 
				(GalaxyGenerator.TypeSpiral)options.ItemAtKey<IValue<int>>("Type").Value, 
				GUILayout.MaxWidth(75));

			/*EditorGUILayout.Foldout(true, "Archimedean");

			options["SpiralArchimedeanCenter"].Edit("Center", GUILayout.MaxWidth(75));
			options["SpiralArchimedeanDistance"].Edit("Distance", GUILayout.MaxWidth(75));
			options["SpiralArchimedeanTwist"].Edit("Twist", GUILayout.MaxWidth(75));

			EditorGUILayout.Foldout(true, "Logarythmic");

			options["SpiralLogarithmicCenter"].Edit("Center", GUILayout.MaxWidth(75));
			options["SpiralLogarithmicDistance"].Edit("Distance", GUILayout.MaxWidth(75));*/

			//SceneView.RepaintAll();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
