using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Galaxeed.Options
{
	[Serializable]
	public class SeedOptionItemFloat : SeedOptionItem<float>
	{
		public override float GetRandom()
		{
			return Random.Range(this.Min, this.Max);
		}

		public override void Edit(string name, params GUILayoutOption[] options)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(name, options);
			this.Value = EditorGUILayout.Slider(
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
			throw new NotImplementedException();
		}

		public override float ToFloat()
		{
			return this._value;
		}
	}
}
