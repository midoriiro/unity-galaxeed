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
	public abstract class OptionEditor<TValue> : ScriptableObject
	{
		[SerializeField]
		private TValue _item;
		public virtual TValue Item
		{
			get
			{
				return this._item;
			}
			set
			{
				this._item = value;
			}
		}

		public bool IsInline { get; set; }
		public bool IsLock { get; set; }

		[SerializeField]
		private GUILayoutOption[] _options;
		public GUILayoutOption[] Options
		{
			get
			{
				return this._options;
			}
			set
			{
				this._options = value;
			}
		}

		[SerializeField]
		private GUIStyle _style;
		public GUIStyle Style
		{
			get
			{
				return this._style;
			}
			set
			{
				this._style = value;
			}
		}

		[SerializeField]
		private string _label;
		public string Label
		{
			get
			{
				return this._label;
			}
			set
			{
				this._label = value;
			}
		}

		protected Rect? Surface;

		protected virtual void OnEnable()
		{
		}

		protected void BeginEdit()
		{
			this.Surface = null;

			if (this.IsInline)
			{
				if (this._style != null && this._options != null)
					this.Surface = EditorGUILayout.BeginHorizontal(this._style, this._options);
				else if (this._style == null && this._options != null)
					this.Surface = EditorGUILayout.BeginHorizontal(this._options);
				else
					this.Surface = EditorGUILayout.BeginHorizontal();
			}

			this.IsLock = EditorGUILayout.Toggle(this.IsLock, "IN LockButton");

			if (this.IsLock)
				EditorGUI.BeginDisabledGroup(this.IsLock);

			var item = (IItem)this.Item;

			item.IsDirty = EditorGUILayout.Toggle(item.IsDirty);

			if (this._label != null || this._label != "")
				EditorGUILayout.LabelField(name, _options);
		}

		protected void EndEdit()
		{
			if (this.IsLock)
				EditorGUI.EndDisabledGroup();

			if (this.IsInline)
				EditorGUILayout.EndHorizontal();

			this.Surface = null;
		}

		protected abstract void Edit();

		//public void Edit(GUILayoutOption[] options)
		//{
		//	this.options = options;
		//	this.style = null;
		//	this.label = null;

		//	this.Edit();
		//}

		//public void Edit(GUIStyle style)
		//{
		//	this.options = null;
		//	this.style = style;
		//	this.label = null;

		//	this.Edit();
		//}

		//public void Edit(string label)
		//{
		//	this.options = null;
		//	this.style = null;
		//	this.label = label;
		//}

		//public void Edit(GUILayoutOption[] options, GUIStyle style)
		//{
		//	this.options = options;
		//	this.style = style;
		//	this.label = null;
		//}

		//public void Edit(string label, GUILayoutOption[] options)
		//{
		//	this.options = options;
		//	this.style = null;
		//	this.label = label;
		//}

		//public void Edit(string label, GUIStyle style)
		//{
		//	this.options = null;
		//	this.style = style;
		//	this.label = label;
		//}

		//public void Edit(string label, GUILayoutOption[] options, GUIStyle style)
		//{
		//	this.options = options;
		//	this.style = style;
		//	this.label = label;
		//}
	}
}
