using Galaxeed.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Options
{
	[Serializable]
	public abstract class SeedOptionItem<TValue> : OptionItem<TValue>, ISeedOptionItem, IRandom<TValue>
	{
		public bool IsRandom { get; set; }

		public override TValue Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (this._value.Equals(value)) return;

				if (!this.IsRandom)
					this.DefaultValue = value;

				this._value = value;
				this.IsDirty = true;

				if (this.Options != null)
					this.Options.OnOptionItemChanged(new EventArgs());
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			this.IsRandom = true;
		}

		public abstract TValue GetRandom();
	}
}
