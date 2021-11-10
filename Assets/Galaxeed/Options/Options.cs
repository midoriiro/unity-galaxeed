using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Galaxeed.Options
{
	[Serializable]
	public class Options : ScriptableObject, IOptions
	{
		public List<ScriptableObject> Items;

		[SerializeField]
		private bool _dirty;
		public bool IsDirty
		{
			get { return this._dirty; }

			set
			{
				if (value == true || this._dirty == false) return;

				foreach (var item in this.Items)
				{
					//item.Reset();
				}

				this._dirty = false;
			}
		}

		public event EventHandler OptionItemChanged;

		public virtual void OnOptionItemChanged(EventArgs e)
		{
			if (this.OptionItemChanged != null)
				this.OptionItemChanged(this, e);
		}

		protected virtual void OnEnable()
		{
			this._dirty = false;

			if (this.Items == null)
				this.Items = new List<ScriptableObject>();
		}

		protected virtual void OnDestroy()
		{
			foreach (var item in this.Items)
				DestroyImmediate(item);
		}

		public virtual void Add(IItem item)
		{
			if (this.Contains(item.Name))
				throw new Exception("Item already exists");

			item.Options = this;
			this.Items.Add((ScriptableObject)item);

			item.Initialyze();
		}

		public void AddRange(IItem[] items)
		{
			foreach (var item in items)
				this.Add(item);
		}

		private bool Contains(string key)
		{
			foreach (var item in this.Items)
			{
				var i = (IItem)item;

				if (i.Name == key)
					return true;
			}

			return false;
		}

		public T ItemAtKey<T>(string key)
		{
			IItem result = null;

			foreach(var item in this.Items)
			{
				var i = (IItem)item;

				if (i.Name == key)
				{
					result = i;
					break;
				}
			}

			return (T)result;
		}
	}
}