using System;
using UnityEditor;
using UnityEngine;

namespace Galaxeed.Options
{
	[Serializable]
	public class SeedValue : ScriptableObject, IEditable
	{
		private SeedOptions _options;

		public int Min;
		public int Max;

		public event EventHandler SeedChanged;

		[SerializeField]
		private int _value;
		/*public int Value
		{
			get { return this._value; }

			set
			{

				//if (this._value == value) return;

				//this._value = value;

				UnityEngine.Random.InitState(this._value);

				foreach (var item in this._options.Items)
				{
					if (item.IsRandom)
					{
						item.ItemValue.Value = item.GetRandom();
						item.ItemValue.DefaultValue = item.ItemValue.Value;
					}

					item.IsDirty = false;
				}

				if (this.SeedChanged != null)
					this.SeedChanged(this, new EventArgs());
			}
		}*/

		public void Initialyze(SeedOptions options)
		{
			this._options = options;
			this.Min = int.MinValue;
			this.Max = int.MaxValue;
		}

		public void SetValue(int value)
		{
			this._value = value;
		}

		public int GetValue()
		{
			return this._value;
		}

		public void Edit(string name, params GUILayoutOption[] options)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(name, options);
			this._value = EditorGUILayout.IntSlider(
				this._value,
				this.Min,
				this.Max);
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Random Seed"))
			{
				this._value = UnityEngine.Random.Range(this.Min, this.Max);
			}
		}

		public void Edit(string name, Enum selected, params GUILayoutOption[] options)
		{
			throw new NotImplementedException();
		}
	}
}
