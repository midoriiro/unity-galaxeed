using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxeed.Options
{
	public interface IValue<TValue>
	{
		TValue DefaultValue { get; set; }
		TValue Value { get; set; }

		void Reset();
	}
}
