using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Galaxeed.Options
{
	[Serializable]
	public class SeedOptionItemInt : SeedOptionItem<int>
	{
		public override int GetRandom()
		{
			return Random.Range(this.Min, this.Max);
		}

		public override void Edit(string name, params GUILayoutOption[] options)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(name, options);
			this.Value = EditorGUILayout.IntSlider(
				this.Value,
				this.Min,
				this.Max);
			EditorGUILayout.EndHorizontal();
		}

		public override void Edit(string name, Enum selected, params GUILayoutOption[] options)
		{
			throw new NotImplementedException();
		}

		public override int ToInt()
		{
			return this._value;
		}

		public override float ToFloat()
		{
			throw new NotImplementedException();
		}
	}
}
