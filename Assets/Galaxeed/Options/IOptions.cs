using System;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxeed.Options
{
	public interface IOptions
	{
		bool IsDirty { get; set; }

		event EventHandler OptionItemChanged;
		void OnOptionItemChanged(EventArgs e);

		void Add(IItem item);
		void AddRange(IItem[] items);
	}
}