using Galaxeed.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Options
{
	[Serializable]
	public abstract class OptionItem<TValue> : ScriptableObject, IItem, IEditable, IConvertible, IValue<TValue>
	{
		[SerializeField]
		private Options _options;
		public Options Options
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
		private OptionEditor<TValue> _editor;
		public OptionEditor<TValue> Editor
		{
			get
			{
				return this._editor;
			}
			set
			{
				this.Editor = value;
			}
		}

		[SerializeField]
		private string _name;
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public bool IsDirty { get; set; }

		[SerializeField]
		private TValue _min;
		public TValue Min
		{
			get
			{
				return this._min;
			}
			set
			{
				this._min = value;
			}
		}

		[SerializeField]
		private TValue _max;
		public TValue Max
		{
			get
			{
				return this._max;
			}
			set
			{
				this._max = value;
			}
		}

		public TValue DefaultValue { get; set; }

		[SerializeField]
		protected TValue _value;
		public virtual TValue Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (this._value.Equals(value)) return;

				this.DefaultValue = value;

				this._value = value;
				this.IsDirty = true;

				if (this.Options != null)
					this.Options.OnOptionItemChanged(new EventArgs());
			}
		}

		protected virtual void OnEnable()
		{
			this.IsDirty = false;
		}

		public void Initialyze()
		{
			this._value = this.Min;
			this.DefaultValue = this._value;
		}

		public void Reset()
		{
			//this._value = this.DefaultValue;
			this.IsDirty = false;
		}

		public abstract void Edit(string name, params GUILayoutOption[] options);
		public abstract void Edit(string name, Enum selected, params GUILayoutOption[] options);

		public abstract int ToInt();
		public abstract float ToFloat();
	}
}
