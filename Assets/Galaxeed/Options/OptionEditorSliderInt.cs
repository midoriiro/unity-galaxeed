using Galaxeed.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Galaxeed.Options
{
	[Serializable]
	public abstract class OptionEditorSliderInt : OptionEditor<OptionItem<int>>
	{
		protected override void Edit()
		{
			this.BeginEdit();

			this.Item.Value = EditorGUILayout.IntSlider(
				this.Item.Value,
				this.Item.Min,
				this.Item.Max);

			this.EndEdit();
		}
	}
}
