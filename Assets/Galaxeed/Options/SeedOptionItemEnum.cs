using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Galaxeed.Options
{
	[Serializable]
	public class SeedOptionItemEnum : SeedOptionItemInt
	{
		public override void Edit(string name, Enum selected, params GUILayoutOption[] options)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(name, options);
			var value = EditorGUILayout.EnumPopup(selected);
			this.Value = (int)Convert.ChangeType(value, selected.GetType());
			EditorGUILayout.EndHorizontal();
		}
	}
}
